import { Component, OnInit, OnDestroy, Input, ViewChild, HostListener } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterJamMyJamService } from '../../../../services/letterjam/myJam';
import { MyJamRound, IMyJamLetterCard } from '../../../../models/letterjam/myJam';
import _ from 'lodash';
import { ILetterJamClueComponentParameters } from '../clue/clue.component';
import { AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IMyLettersComponentParameters } from './my-letters/my-letters.component';

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
  MyLetters: IMyJamLetterCard[];
  CurrentLetterIndex: number;

  constructor(private myJamService: LetterJamMyJamService,
    private modalService: NgbModal) {
    super();
  }

  ngOnInit(): void {
    this.parameters.hubConnection.on("giveClue", this.clueGiven);
    this.load();
  }
  ngOnDestroy(): void {
    this.parameters.hubConnection.off("giveClue", this.clueGiven);
  }

  getClueComponentProperties = (round: MyJamRound) => {
    return <ILetterJamClueComponentParameters>{
      clue: {
        id: round.clueId,
        letters: []
      },
      getCardsFromCache: this.parameters.getCardsFromCache,
      getPlayersFromCache: this.parameters.getPlayersFromCache,
      request: this.parameters.request
    }
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
            letters: [],
            loadingLetters: true,
            loadingPlayer: true,
            player: null
          };

          this.Rounds.push(roundViewModel);
        });

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

  clueGiven = (clueId: string, clueGiverPlayerId: string) => {
    var roundViewModel = <MyJamRound>{
      clueGiverPlayerId: clueGiverPlayerId,
      clueId: clueId,
      letters: [],
      loadingLetters: true,
      loadingPlayer: true,
      player: null
    };

    this.Rounds.push(roundViewModel);
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

    var letterGuessRequest = myLetters.map(l => {
      return {
        cardId: l.cardId,
        playerLetterGuess: l.playerLetterGuess
      }
    });

    this.myJamService.PostLetterGuesses(this.parameters.request, letterGuessRequest, this.shouldMoveOnToNextLetter)
      .then(response => this.HandleGenericResponseBase(response, () => {
        this.MyLetters = myLetters;
        this.shouldMoveOnToNextLetter = false;
        this.myLettersModalRef.close();

        return response;
      }))
      .catch(this.HandleGenericError);
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
    return index > currentLetterIndex;
  }
}
