import { Component, OnInit, OnDestroy, Input, ViewChild, HostListener, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterJamMyJamService } from '../../../../services/letterjam/myJam';
import { MyJamRound, IMyJamLetterCard, IMyJamRound } from '../../../../models/letterjam/myJam';
import _ from 'lodash';
import { ILetterJamClueComponentParameters } from '../clue/clue.component';
import { AllPlayersFromCacheRequest, PlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IMyLettersComponentParameters } from './my-letters/my-letters.component';
import { GetColourFromLetterIndex, StyleLetterCardWithColour } from '../../../../services/letterjam/colour';
import { ILetterCard, IBonusLetterGuess } from '../../../../models/letterjam/letters';
import { RoundStatusEnum } from '../../../../models/letterjam/clues';

export interface IMyJamComponentParameters extends ITableComponentParameters {
  currentRoundStatus: () => RoundStatusEnum;
}

@Component({
  selector: 'letterjam-my-jam',
  templateUrl: './my-jam.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './my-jam.component.scss']
})
export class LetterJamMyJamComponent extends TableComponentBase implements OnInit, OnDestroy {

  @ViewChild('myLettersModal') myLettersModal;
  @ViewChildren('rounds') rounds: QueryList<ElementRef>;
  myLettersModalRef: NgbModalRef;
  myLettersModalParameters: IMyLettersComponentParameters;

  shouldMoveOnToNextLetter: boolean;

  @Input() parameters: IMyJamComponentParameters;

  Rounds: MyJamRound[] = [];
  FilteredRounds: MyJamRound[] = [];
  CurrentRound: MyJamRound;
  MyLetters: IMyJamLetterCard[] = [];
  FinalWordLetters: IMyJamLetterCard[];
  CurrentLetterIndex: number;
  OnlyShowCluesForMe: boolean = true;
  ShowEmojis: boolean = true;
  DisableEdit: boolean = true;

  constructor(private myJamService: LetterJamMyJamService,
    private modalService: NgbModal) {
    super();
  }

  ngOnInit(): void {
    this.parameters.hubConnection.on("giveClue", this.clueGiven);
    this.parameters.hubConnection.on("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.on("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.on("bonusLetterGuessed", this.onBonusLetterGuessed);
    this.parameters.hubConnection.on('gameEndTriggered', this.onGameEndTriggered);
    this.parameters.hubConnection.on('endGame', this.onGameEnd);
    this.load();
  }
  ngOnDestroy(): void {
    this.parameters.hubConnection.off("giveClue", this.clueGiven);
    this.parameters.hubConnection.off("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.off("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.off('gameEndTriggered', this.onGameEndTriggered);
    this.parameters.hubConnection.off('endGame', this.onGameEnd);
  }

  getClueComponentProperties = (round: MyJamRound) => {
    return <ILetterJamClueComponentParameters>{
      clue: {
        id: round.clueId,
        letters: []
      },
      getCardsFromCache: this.parameters.getCardsFromCache,
      getPlayersFromCache: this.parameters.getPlayersFromCache,
      request: this.parameters.request,
      highlightColour: round.requestingPlayerReceivedClue ? GetColourFromLetterIndex(round.letterIndex) : null,
      showEmojis: this.ShowEmojis
    }
  }

  LetterStyle = (letter: IMyJamLetterCard, index: number) => {
    if (letter.bonusLetter) {
      return {};
    }
    var color = GetColourFromLetterIndex(index + 1);
    return StyleLetterCardWithColour(color);
  }

  load = () => {
    this.myJamService.LoadMyJam(this.parameters.request)
      .then(response => this.parameters.handleGenericResponse(response, r => {

        this.MyLetters = r.myLetters;
        this.CurrentLetterIndex = r.currentLetterIndex;

        _.each(r.rounds, round => {

          var roundViewModel = <MyJamRound>{
            clueGiverPlayerId: round.clueGiverPlayerId,
            clueId: round.clueId,
            requestingPlayerReceivedClue: round.requestingPlayerReceivedClue,
            letters: round.letters,
            loadingLetters: true,
            loadingPlayer: true,
            player: null
          };
          if (round.requestingPlayerReceivedClue) {
            var letter = _.find(round.letters, l => l.playerId == this.parameters.request.PlayerId);
            var indexOf = _.findIndex(this.MyLetters, l => l.cardId == letter.cardId);
            if (indexOf >= 0) {
              roundViewModel.letterIndex = indexOf + 1;
            }
          }

          this.Rounds.push(roundViewModel);
        });

        this.FilterRounds();
        setTimeout(() => {
          this.focusCurrentRound();
        }, 500);

        return response;
      }))
      .then(response => this.parameters.handleGenericResponseBase(response, () => {
        //this.subscribe();
        return this.loadPlayers().then(() => response);
      }))
      .finally(() => {
        this.DisableEdit = this.parameters.currentRoundStatus() == RoundStatusEnum.GameEnd;
      });
  }

  loadPlayers = () => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true)).then(r => {
      _.each(this.Rounds, round => {
        round.player = _.find(r, fP => fP.id == round.clueGiverPlayerId);
        round.loadingPlayer = false;
      });
    });
  }

  clueGiven = (round: IMyJamRound) => {
    var roundViewModel = <MyJamRound>{
      clueGiverPlayerId: round.clueGiverPlayerId,
      clueId: round.clueId,
      requestingPlayerReceivedClue: round.requestingPlayerReceivedClue,
      letters: round.letters,
      loadingLetters: true,
      loadingPlayer: true,
      player: null
    };

    var letter = _.find(round.letters, l => l.playerId == this.parameters.request.PlayerId);
    if (letter) {
      roundViewModel.requestingPlayerReceivedClue = true;
      var indexOf = _.findIndex(this.MyLetters, l => l.cardId == letter.cardId);
      if (indexOf >= 0) {
        roundViewModel.letterIndex = indexOf + 1;
      }
    }
    this.Rounds.push(roundViewModel);

    this.parameters.getPlayersFromCache(new PlayersFromCacheRequest([roundViewModel.clueGiverPlayerId], true, false)).then(r => {
      var player = r[0];
      roundViewModel.player = player;
      roundViewModel.loadingPlayer = false;
      this.FilterRounds();

      setTimeout(() => {
        this.focusCurrentRound();
      }, 300);
    });
  }

  focusCurrentRound = () => {
    if (this.CurrentRound) {      
      window.scrollTo(0, document.body.scrollHeight);
    }
  }

  onPlayerMovedOnToNextCard = (playerId: string, nextCard: ILetterCard) => {
    if (playerId == this.parameters.request.PlayerId) {
      if (nextCard) {
        this.CurrentLetterIndex = _.findIndex(this.MyLetters, c => c.cardId === nextCard.cardId);
        if (this.CurrentLetterIndex < 0) {
          this.CurrentLetterIndex = null;
        }
      }
      else
      {
        this.CurrentLetterIndex = null;
      }
    }
  }
  onNewBonusCard = (newBonusCard: ILetterCard) => {
    if (newBonusCard.playerId === this.parameters.request.PlayerId) {
      this.MyLetters.push({
        bonusLetter: true,
        cardId: newBonusCard.cardId,
        playerLetterGuess: null,
        isWildCard: false
      });
    }
  }
  onBonusLetterGuessed = (bonusLetterGuess: IBonusLetterGuess) => {
    if (bonusLetterGuess.playerId === this.parameters.request.PlayerId) {
      this.MyLetters.pop();
    }
  }
  onGameEndTriggered = () => {
    var unGuessedBonusLetterIndex = _.findIndex(this.MyLetters, l => l.bonusLetter);
    if (unGuessedBonusLetterIndex >= 0) {
      this.MyLetters.splice(unGuessedBonusLetterIndex, 1);
    }
    this.CurrentLetterIndex = null;
    this.shouldMoveOnToNextLetter = false;

    this.EditMyLetters(true);    
  }

  onGameEnd = () => {
    this.DisableEdit = true;
  }

  moveOnToNextLetter = () => {
    this.shouldMoveOnToNextLetter = true;
  }

  EditMyLetters = (gameEnd: boolean = null, editLettersError = null) => {

    this.myLettersModalParameters = {
      myLetters: this.MyLetters.map(l => {
        return {
          ...l
        }
      }),
      finalWordLetters: this.FinalWordLetters != null ? this.FinalWordLetters.map(l => {
        return {
          ...l
        }
      }) : null,
      currentLetterIndex: this.CurrentLetterIndex,
      moveOnToNextLetter: this.moveOnToNextLetter,
      gameEnd: gameEnd != null ? gameEnd : this.parameters.currentRoundStatus() == RoundStatusEnum.GameEndTriggered,
      sessionInfo: this.parameters.request,
      error: editLettersError,
      hubConnection: this.parameters.hubConnection
    }

    const modalState = {
      modal: true,
      desc: 'fake state for our modal'
    };
    history.pushState(modalState, null);
    var modalRef = this.modalService.open(this.myLettersModal, { ariaLabelledBy: 'modal-basic-title' });
    this.myLettersModalRef = modalRef;

    modalRef.result
      .finally(() => {
        this.myLettersModalParameters = null;
        this.myLettersModalRef = null;
        this.onGiveClueModalDismissed();
      });
  }

  UpdateLetterGuesses() {

    var myLetters = this.myLettersModalParameters.myLetters;
    var finalWordLetters = this.myLettersModalParameters.finalWordLetters;
    var originalLetters = this.MyLetters;

    var letterGuessRequest = myLetters.map(l => {
      return {
        cardId: l.cardId,
        playerLetterGuess: l.playerLetterGuess
      }
    });
    var finalLettersRequest = finalWordLetters != null ? finalWordLetters.map(l => {
      return {
        cardId: l.cardId,
        isWildCard: l.isWildCard
      }
    }) : null;

    this.myLettersModalRef.close();

    this.MyLetters = myLetters;

    var promise = this.parameters.currentRoundStatus() == RoundStatusEnum.GameEndTriggered ?
      this.myJamService.PostFinalWord(this.parameters.request, letterGuessRequest, finalLettersRequest) :
      this.myJamService.PostLetterGuesses(this.parameters.request, letterGuessRequest, this.shouldMoveOnToNextLetter);

    promise
      .then(response => {

        if (!response.success) {
          var error = "";
          if (response.errorCode == "9566FE53-76E7-4F81-A295-9052D7C03CA8") {
            error = "Another player has already reserved the wildcard";
          }
          else if (response.errorCode == "C0FBCE81-207F-4DE6-A953-1D9F66AA9279") {
            error = "Another player has already reserved a bonus letter you chose";
          }
          else {
            error = response.errorCode;
          }
          this.EditMyLetters(this.parameters.currentRoundStatus() == RoundStatusEnum.GameEndTriggered, error);
        }

        return response;
      })
      .then(response => this.parameters.handleGenericResponseBase(response, () => {        
        this.shouldMoveOnToNextLetter = false;

        return response;
      }))
      .catch((err) => {
        this.parameters.handleGenericError(err);
        this.MyLetters = originalLetters;
        this.FinalWordLetters = [];
      });
  }

  @HostListener('window:popstate', ['$event'])
  dismissModal(event: Event) {
    if (this.myLettersModalRef) {
      this.myLettersModalRef.dismiss();
      event.stopPropagation();
    }
  }

  onGiveClueModalDismissed = () => {
    if (window.history.state.modal === true) {
      window.history.state.modal = false;
      history.back();
    }
  }

  indexGreaterThanCurrentLetterIndex(index: number, currentLetterIndex: number) {
    return currentLetterIndex != null && index > currentLetterIndex;
  }

  OnToggleCluesForMe = () => {
    var onlyShowCluesForMe = !this.OnlyShowCluesForMe;
    this.OnlyShowCluesForMe = onlyShowCluesForMe;

    this.FilterRounds();
  }

  FilterRounds = () => {
    var rounds = this.Rounds;

    if (this.parameters.currentRoundStatus() == RoundStatusEnum.ReceivedClue) {
      this.CurrentRound = _.last(rounds);
    }
    else {
      this.CurrentRound = null;
    }

    var filteredRounds = rounds;
    if (this.OnlyShowCluesForMe) {
      filteredRounds  = _.filter(rounds, (r, index) => {
        if (index == filteredRounds.length - 1) {
          return true;
        }
        if (r.clueGiverPlayerId == this.parameters.request.PlayerId) {
          return false;
        }
        if (!r.requestingPlayerReceivedClue) {
          return false;
        }

        return true;
      });
    }

    this.FilteredRounds = filteredRounds;
  }
}
