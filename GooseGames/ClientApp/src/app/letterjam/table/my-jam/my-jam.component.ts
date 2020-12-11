import { Component, OnInit, OnDestroy, Input, ViewChild, HostListener } from '@angular/core';
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

export interface IMyJamComponentParameters extends ITableComponentParameters {
  proposeClue: () => void;
}

@Component({
  selector: 'letterjam-my-jam',
  templateUrl: './my-jam.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './my-jam.component.scss']
})
export class LetterJamMyJamComponent extends TableComponentBase implements OnInit, OnDestroy {

  @ViewChild('myLettersModal') myLettersModal;
  myLettersModalRef: NgbModalRef;
  myLettersModalParameters: IMyLettersComponentParameters;

  shouldMoveOnToNextLetter: boolean;

  @Input() parameters: IMyJamComponentParameters;

  Rounds: MyJamRound[] = [];
  FilteredRounds: MyJamRound[] = [];
  MyLetters: IMyJamLetterCard[];
  CurrentLetterIndex: number;
  OnlyShowCluesForMe: boolean = true;

  constructor(private myJamService: LetterJamMyJamService,
    private modalService: NgbModal) {
    super();
  }

  ngOnInit(): void {
    this.parameters.hubConnection.on("giveClue", this.clueGiven);
    this.parameters.hubConnection.on("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.on("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.on("bonusLetterGuessed", this.onBonusLetterGuessed);
    this.load();
  }
  ngOnDestroy(): void {
    this.parameters.hubConnection.off("giveClue", this.clueGiven);
    this.parameters.hubConnection.off("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.off("newBonusCard", this.onNewBonusCard);
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
      highlightColour: round.requestingPlayerReceivedClue ? GetColourFromLetterIndex(round.letterIndex) : null
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
      .then(response => this.HandleGenericResponse(response, r => {

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

        return response;
      }))
      .then(response => this.HandleGenericResponseBase(response, () => {
        //this.subscribe();
        return this.loadPlayers().then(() => response);
      }));
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

    return this.parameters.getPlayersFromCache(new PlayersFromCacheRequest([roundViewModel.clueGiverPlayerId], true, false)).then(r => {
      var player = r[0];
      roundViewModel.player = player;
      roundViewModel.loadingPlayer = false;
      this.FilterRounds();
    });
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
        playerLetterGuess: null
      });
    }
  }
  onBonusLetterGuessed = (bonusLetterGuess: IBonusLetterGuess) => {
    if (bonusLetterGuess.playerId === this.parameters.request.PlayerId) {
      this.MyLetters.pop();
    }
  }

  moveOnToNextLetter = () => {
    this.shouldMoveOnToNextLetter = true;
  }

  EditMyLetters = () => {

    this.myLettersModalParameters = {
      myLetters: this.MyLetters.map(l => {
        return {
          ...l
        }
      }),
      currentLetterIndex: this.CurrentLetterIndex,
      moveOnToNextLetter: this.moveOnToNextLetter
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
    var originalLetters = this.MyLetters;

    var letterGuessRequest = myLetters.map(l => {
      return {
        cardId: l.cardId,
        playerLetterGuess: l.playerLetterGuess
      }
    });

    this.MyLetters = myLetters;
    this.myJamService.PostLetterGuesses(this.parameters.request, letterGuessRequest, this.shouldMoveOnToNextLetter)
      .then(response => this.HandleGenericResponseBase(response, () => {        
        this.shouldMoveOnToNextLetter = false;
        this.myLettersModalRef.close();

        return response;
      }))
      .catch(() => {
        this.HandleGenericError;
        this.MyLetters = originalLetters;
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
    if (this.OnlyShowCluesForMe) {
      this.FilteredRounds = _.filter(this.Rounds, r => {

        if (r.clueGiverPlayerId == this.parameters.request.PlayerId) {
          return false;
        }
        if (!r.requestingPlayerReceivedClue) {
          return false;
        }

        return true;

      });
    }
    else {
      this.FilteredRounds = this.Rounds;
    }
  }
}
