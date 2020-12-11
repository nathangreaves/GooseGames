import { Component, OnInit, ViewChild, OnDestroy, HostListener } from '@angular/core';
import { LetterJamComponentBase, LetterJamPlayerStatus } from '../../../models/letterjam/content';
import { ITableViewParameters } from './table-view/table-view.component';
import { ILetterCard } from '../../../models/letterjam/letters';
import { ICardsRequest } from '../../../models/letterjam/table';
import { LetterJamLetterCardService } from '../../../services/letterjam/letterCard';
import _ from 'lodash';
import { IProposedCluesComponentParameters } from './proposed-clues/proposed-clues.component';
import { IProposeClueComponentParameters } from './propose-clue/propose-clue.component';
import { ITableComponentParameters } from './table-base.component';

import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ILetterJamClueComponentParameters } from './clue/clue.component';
import { LetterJamCluesService } from '../../../services/letterjam/clues';
import { IMyJamComponentParameters } from './my-jam/my-jam.component';
import { RoundStatusEnum } from '../../../models/letterjam/clues';
import { LetterJamTableService } from '../../../services/letterjam/table';
import { LetterJamPlayerStatusService } from '../../../services/letterjam/playerStatus';

export enum TableComponentTabs {
  Table = 0,
  ProposedClues = 1,
  ProposeClue = 2,
  MyJam = 3
}

const LocalStorageTabKey = "goose-games-letter-jam-table-tab";

@Component({
  selector: 'letterjam-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss']
})
export class LetterJamTableComponent extends LetterJamComponentBase implements OnInit, OnDestroy {

  @ViewChild('giveClueModal') giveClueModal;

  tableViewParameters: ITableViewParameters;
  proposedCluesParameters: IProposedCluesComponentParameters;
  proposeClueParameters: IProposeClueComponentParameters;
  myJamParameters: IMyJamComponentParameters;

  letterCards: ILetterCard[] = [];
  CurrentRoundId: string;

  CurrentTabId: number = null;
  TableLoaded: boolean;
  ProposedCluesLoaded: boolean;
  ProposeClueLoaded: boolean;
  MyJamLoaded: boolean;

  PlayerStatus: LetterJamPlayerStatus;
  RoundStatus: RoundStatusEnum;  
  DisableNextRoundButton: boolean;

  clueModalParameters: ILetterJamClueComponentParameters;
  giveClueModalRef: NgbModalRef;

  constructor(private letterCardService: LetterJamLetterCardService,
    private clueService: LetterJamCluesService,
    private tableService: LetterJamTableService,
    private playerStatusService: LetterJamPlayerStatusService,
    private modalService: NgbModal) {
    super();
  }

  Table = () => {
    this.TableLoaded = true;
    this.CurrentTabId = TableComponentTabs.Table;
    this.setTabIdInLocalStorage();
  }

  ProposedClues = () => {
    this.ProposedCluesLoaded = true;
    this.CurrentTabId = TableComponentTabs.ProposedClues;
    this.setTabIdInLocalStorage();
  }

  ProposeClue = () => {
    this.ProposeClueLoaded = true;
    this.CurrentTabId = TableComponentTabs.ProposeClue;
    this.setTabIdInLocalStorage();
  }

  MyJam = () => {
    this.MyJamLoaded = true;
    this.CurrentTabId = TableComponentTabs.MyJam;
    this.setTabIdInLocalStorage();
  }

  ReadyForNextRound = () => {
    this.DisableNextRoundButton = true;
    this.playerStatusService.SetWaitingForNextRound(this)
      .then(response => this.HandleGenericResponseBase(response, () => {

        this.PlayerStatus = LetterJamPlayerStatus.ReadyForNextRound;
        this.Table();

        return response;
      }))
      .finally(() => {
        this.DisableNextRoundButton = false;
      });
  }

  NotReadyForNextRound = () => {
    this.DisableNextRoundButton = true;
    this.playerStatusService.SetUndoWaitingForNextRound(this)
      .then(response => this.HandleGenericResponseBase(response, () => {

        this.PlayerStatus = LetterJamPlayerStatus.ReceivedClue;
        this.MyJam();

        return response;
      }))
      .finally(() => {
        this.DisableNextRoundButton = false;
      });
  }

  private setTabIdInLocalStorage() {
    localStorage.setItem(LocalStorageTabKey, this.CurrentTabId.toString());
  }

  ngOnInit(): void {

    var baseTableParameters = <ITableComponentParameters>{
      request: this,
      getCardsFromCache: this.getCardsFromCache,
      getPlayersFromCache: this.GetPlayersFromCache,
      setCurrentRoundId: this.setCurrentRoundId,
      getCurrentRoundId: this.getCurrentRoundId,
      hubConnection: this.HubConnection
    }

    this.HubConnection.on('promptGiveClue', this.promptGiveClue);
    this.HubConnection.on('giveClue', this.clueGiven);
    this.HubConnection.on('beginNewRound', this.beginNewRound);

    this.tableViewParameters = <ITableViewParameters>{
      ...baseTableParameters
    };
    this.proposedCluesParameters = <IProposedCluesComponentParameters>{
      ...baseTableParameters,
      proposeClue: this.ProposeClue
    };
    this.proposeClueParameters = <IProposeClueComponentParameters>{
      ...baseTableParameters,
      proposedClues: this.ProposedClues
    }
    this.myJamParameters = <IMyJamComponentParameters>{
      ...baseTableParameters
    }

    var tabItem = localStorage.getItem(LocalStorageTabKey);
    if (tabItem !== null && tabItem !== undefined) {
      this.CurrentTabId = parseInt(tabItem);
    }

    switch (this.CurrentTabId) {
      case TableComponentTabs.ProposeClue:
        this.ProposeClueLoaded = true;
        break;
      case TableComponentTabs.ProposedClues:
        this.ProposedCluesLoaded = true;
        break;
      case TableComponentTabs.MyJam:
        this.MyJamLoaded = true;
        break;
      default:
        this.CurrentTabId = TableComponentTabs.Table;
        this.TableLoaded = true;
        break;
    }

    this.RefreshCache();
    this.getRelevantLetters();
    this.loadRound();
  }

  ngOnDestroy() {
    this.HubConnection.off('promptGiveClue', this.promptGiveClue);
    this.HubConnection.off('giveClue', this.clueGiven);
    this.HubConnection.off('beginNewRound', this.beginNewRound);
  }

  loadRound() {
    this.tableService.GetCurrentRound(this)
      .then(response => this.HandleGenericResponse(response, r => {

        this.RoundStatus = r.roundStatus;
        this.setCurrentRoundId(r.roundId);
        this.PlayerStatus = LetterJamPlayerStatus[r.playerStatus];

        return response;
      }));
  }

  promptGiveClue = (clueGiverId: string, clueId: string) => {
    if (clueGiverId === this.PlayerId) {
      this.clueModalParameters = <ILetterJamClueComponentParameters>{
        request: this,
        getCardsFromCache: this.getCardsFromCache,
        getPlayersFromCache: this.GetPlayersFromCache,
        clue: {
          id: clueId,
          letters: []
        },
        highlightColour: null
      };

      const modalState = {
        modal: true,
        desc: 'fake state for our modal'
      };
      history.pushState(modalState, null);
      var modalRef = this.modalService.open(this.giveClueModal, { ariaLabelledBy: 'modal-basic-title' });
      this.giveClueModalRef = modalRef;

      modalRef.result
        .then(reason => {
          if (reason != "ClueGiven") {
            this.undoClueVote();
          }
        })
        .catch(() => {
          this.undoClueVote();
        })
        .finally(() => {
          this.clueModalParameters = null;
          this.giveClueModalRef = null;
          this.onGiveClueModalDismissed();
        });
    }
  }

  @HostListener('window:popstate', ['$event'])
  dismissModal(event: Event) {
    if (this.modalService.hasOpenModals()) {
      this.modalService.dismissAll();
      event.stopPropagation();
    }
  }

  onGiveClueModalDismissed = () => {
    if (window.history.state.modal === true) {
      window.history.state.modal = false;
      history.back();
    }
  }   

  undoClueVote = () => {
    this.clueService.Vote(this, this.getCurrentRoundId(), null);
  }

  dismissGiveClueModal = () => {
    this.giveClueModalRef.dismiss();
  }

  giveClue = () => {
    this.clueService.GiveClue(this, this.getCurrentRoundId(), this.clueModalParameters.clue.id).then(() => {
      this.giveClueModalRef.close("ClueGiven");
    });
  }

  beginNewRound = (roundId: string) => {
    this.setCurrentRoundId(roundId);
    this.RoundStatus = RoundStatusEnum.ProposingClues;
    this.PlayerStatus = LetterJamPlayerStatus.ProposingClues;
  }

  clueGiven = () => {
    if (this.CurrentTabId !== TableComponentTabs.MyJam) {
      this.MyJam();
    }
    this.RoundStatus = RoundStatusEnum.ReceivedClue;
    this.PlayerStatus = LetterJamPlayerStatus.ReceivedClue;
  }

  getCurrentRoundId = (): string => {
    return this.CurrentRoundId;
  }
  setCurrentRoundId = (currentRoundId: string): void => {
    this.CurrentRoundId = currentRoundId;
  }

  getRelevantLetters(): Promise<ILetterCard[]> {
    return this.letterCardService.GetRelevantLetters(this)
      .then(response => {
        if (response.success) {
          _.each(response.data, this.addLetterCardToCache);

          return response.data;
        }
        this.SetErrorMessage(response.errorCode);
        return [];
      })
      .catch(err => {
        this.HandleGenericError(err);
        return [];
      });
  }

  getCardsFromCache = (request: ICardsRequest): Promise<ILetterCard[]> => {

    if (request.relevantCards) {
      return this.getRelevantLetters();
    }

    var response = [];
    var newRequest = <ICardsRequest>{
      cardIds: []
    }
    _.each(request.cardIds, requestedCardId => {

      var found = _.find(this.letterCards, lC => lC.cardId == requestedCardId);
      if (found) {
        response.push(found);
      }
      else {
        newRequest.cardIds.push(requestedCardId);
      }
    });

    if (newRequest.cardIds.length > 0) {
      return this.letterCardService.GetLetters(this, newRequest).then(res => this.HandleGenericResponse(res, r => {
        _.each(r, lC => {
          this.addLetterCardToCache(lC);
          response.push(lC);
        });
        return res;
      })).then(() => {
        return response;
      });
    }

    return Promise.resolve(response);
  };

  addLetterCardToCache = (letterCard: ILetterCard) => {
    var find = _.find(this.letterCards, lC => lC.cardId === letterCard.cardId);
    if (!find) {
      this.letterCards.push(letterCard);
    }
  }
}
