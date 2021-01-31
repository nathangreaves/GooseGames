import { Component, OnInit, ViewChild, OnDestroy, HostListener } from '@angular/core';
import { LetterJamComponentBase, LetterJamPlayerStatus, PlayersFromCacheRequest } from '../../../models/letterjam/content';
import { ITableViewParameters } from './table-view/table-view.component';
import { ILetterCard, IBonusLetterGuess } from '../../../models/letterjam/letters';
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
import { ILetterJamBonusLetterGuessedParameters } from './bonus-letter-guessed/bonus-letter-guessed.component';
import { ILetterJamGameEndComponentParameters } from './game-end/game-end.component';

export enum TableComponentTabs {
  Table = 0,
  ProposedClues = 1,
  ProposeClue = 2,
  MyJam = 3,
  GameEnd = 4
}

const LocalStorageTabKey = "goose-games-letter-jam-table-tab";

@Component({
  selector: 'letterjam-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss']
})
export class LetterJamTableComponent extends LetterJamComponentBase implements OnInit, OnDestroy {

  @ViewChild('giveClueModal') giveClueModal;
  @ViewChild('bonusLetterGuessedModal') bonusLetterGuessedModal;

  tableViewParameters: ITableViewParameters;
  proposedCluesParameters: IProposedCluesComponentParameters;
  proposeClueParameters: IProposeClueComponentParameters;
  myJamParameters: IMyJamComponentParameters;
  gameEndParameters: ILetterJamGameEndComponentParameters;

  letterCards: ILetterCard[] = [];
  CurrentRoundId: string;

  CurrentTabId: number = null;
  TableLoaded: boolean;
  ProposedCluesLoaded: boolean;
  ProposeClueLoaded: boolean;
  MyJamLoaded: boolean;
  GameEndLoaded: boolean;

  PlayerStatus: LetterJamPlayerStatus;
  RoundStatus: RoundStatusEnum;  
  DisableNextRoundButton: boolean;

  clueModalParameters: ILetterJamClueComponentParameters;
  giveClueModalRef: NgbModalRef;

  bonusLetterGuessedModalParameters: ILetterJamBonusLetterGuessedParameters;
  bonusLetterGuessedModalRef: NgbModalRef;

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
    this.onChangeTab();
  }

  ProposedClues = () => {
    this.ProposedCluesLoaded = true;
    this.CurrentTabId = TableComponentTabs.ProposedClues;
    this.onChangeTab();
  }

  ProposeClue = () => {
    this.ProposeClueLoaded = true;
    this.CurrentTabId = TableComponentTabs.ProposeClue;
    this.onChangeTab();
  }

  MyJam = () => {
    this.MyJamLoaded = true;
    this.CurrentTabId = TableComponentTabs.MyJam;
    this.onChangeTab();
  }

  GameEnd = () => {
    this.GameEndLoaded = true;
    this.CurrentTabId = TableComponentTabs.GameEnd;
    this.onChangeTab();
  }

  onChangeTab = () => {
    this.setTabIdInLocalStorage();
    this.ClearErrorMessage();
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

  ReadyForGameEnd = () => {
    this.DisableNextRoundButton = true;
    this.playerStatusService.SetWaitingForGameEnd(this)
      .then(response => this.HandleGenericResponseBase(response, () => {

        this.PlayerStatus = LetterJamPlayerStatus.ReadyForGameEnd;
        this.Table();

        return response;
      }))
      .finally(() => {
        this.DisableNextRoundButton = false;
      });
  }

  NotReadyForGameEnd = () => {
    this.DisableNextRoundButton = true;
    this.playerStatusService.SetUndoWaitingForGameEnd(this)
      .then(response => this.HandleGenericResponseBase(response, () => {

        this.PlayerStatus = LetterJamPlayerStatus.SubmittedFinalWord;
        this.MyJam();

        return response;
      }))
      .finally(() => {
        this.DisableNextRoundButton = false;
      });
  }

  private setTabIdInLocalStorage() {
    if (this.RoundStatus !== RoundStatusEnum.GameEnd) {
      localStorage.setItem(LocalStorageTabKey, this.CurrentTabId.toString());
    }
    else {
      localStorage.removeItem(LocalStorageTabKey);
    }
  }

  ngOnInit(): void {

    var baseTableParameters = <ITableComponentParameters>{
      request: this,
      getCardsFromCache: this.getCardsFromCache,
      getPlayersFromCache: this.GetPlayersFromCache,
      setCurrentRoundId: this.setCurrentRoundId,
      getCurrentRoundId: this.getCurrentRoundId,
      hubConnection: this.HubConnection,
      handleGenericError: this.HandleGenericError,
      handleGenericResponse: this.HandleGenericResponse,
      handleGenericResponseBase: this.HandleGenericResponseBase,
      setErrorMessage: this.SetErrorMessage
    };

    this.HubConnection.on('promptGiveClue', this.promptGiveClue);
    this.HubConnection.on('giveClue', this.clueGiven);
    this.HubConnection.on('beginNewRound', this.beginNewRound);
    this.HubConnection.on('bonusLetterGuessed', this.onBonusLetterGuessed);
    this.HubConnection.on('gameEndTriggered', this.onGameEndTriggered);
    this.HubConnection.on('playerStatus', this.onPlayerStatus);
    this.HubConnection.on('endGame', this.onGameEnd);

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
      ...baseTableParameters,
      currentRoundStatus: () => this.RoundStatus
    }
    this.gameEndParameters = <ILetterJamGameEndComponentParameters>{
      ...baseTableParameters
    }

    this.RefreshCache();
    this.getRelevantLetters();
    this.loadRound();
  }

  ngOnDestroy() {
    this.HubConnection.off('promptGiveClue', this.promptGiveClue);
    this.HubConnection.off('giveClue', this.clueGiven);
    this.HubConnection.off('beginNewRound', this.beginNewRound);
    this.HubConnection.off('bonusLetterGuessed', this.onBonusLetterGuessed);
    this.HubConnection.off('gameEndTriggered', this.onGameEndTriggered);
    this.HubConnection.off('playerStatus', this.onPlayerStatus);
    this.HubConnection.off('endGame', this.onGameEnd);
  }

  loadRound() {
    this.tableService.GetCurrentRound(this)
      .then(response => this.HandleGenericResponse(response, r => {

        this.RoundStatus = r.roundStatus;
        this.setCurrentRoundId(r.roundId);
        this.PlayerStatus = LetterJamPlayerStatus[r.playerStatus];

        var tabItem = localStorage.getItem(LocalStorageTabKey);
        if (tabItem !== null && tabItem !== undefined) {
          this.CurrentTabId = parseInt(tabItem);
        }

        if (this.RoundStatus == RoundStatusEnum.GameEnd) {
          this.CurrentTabId = TableComponentTabs.GameEnd;
        }
        else {
          this.CurrentTabId = TableComponentTabs.Table;
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
          case TableComponentTabs.GameEnd:
            this.GameEndLoaded = true;
            break;
          default:
            this.CurrentTabId = TableComponentTabs.Table;
            this.TableLoaded = true;
            break;
        }

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
        highlightColour: null,
        showEmojis: true
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
          this.onModalDismissed();
        });
    }
  }

  @HostListener('window:popstate', ['$event'])
  dismissModal(event: Event) {    
    if (this.giveClueModalRef) {
      this.giveClueModalRef.dismiss();
      event.stopPropagation();
    }
    if (this.bonusLetterGuessedModalRef) {
      this.bonusLetterGuessedModalRef.dismiss();
      event.stopPropagation();
    }
  }

  onModalDismissed = () => {
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

  onBonusLetterGuessed = (bonusLetterGuess: IBonusLetterGuess) => {
    if (bonusLetterGuess.playerId !== this.PlayerId && this.modalService.hasOpenModals()) {
      return;
    }

    this.GetPlayersFromCache(new PlayersFromCacheRequest([bonusLetterGuess.playerId], true, false))
      .then(response => {
        var player = response[0];

        this.bonusLetterGuessedModalParameters = {
          ...bonusLetterGuess,
          player: player,
          sessionInfo: this
        };

        const modalState = {
          modal: true,
          desc: 'fake state for our modal'
        };
        history.pushState(modalState, null);
        var modalRef = this.modalService.open(this.bonusLetterGuessedModal, { ariaLabelledBy: 'modal-basic-title' });
        this.bonusLetterGuessedModalRef = modalRef;

        return modalRef.result
          .then(reason => {
            if (reason != "ClueGiven") {
              this.undoClueVote();
            }
          })
          .catch(() => {
            this.undoClueVote();
          })
          .finally(() => {
            this.bonusLetterGuessedModalParameters = null;
            this.bonusLetterGuessedModalRef = null;
            this.onModalDismissed();
          });
      });

  }

  onGameEndTriggered = () => {
    this.RoundStatus = RoundStatusEnum.GameEndTriggered;
    this.PlayerStatus = LetterJamPlayerStatus.PreparingFinalWord;
  }

  onPlayerStatus = (playerId: string, playerStatus: string) => {
    if (playerId === this.PlayerId) {
      this.PlayerStatus = LetterJamPlayerStatus[playerStatus];
    }
  }

  onGameEnd = () => {
    this.RoundStatus = RoundStatusEnum.GameEnd;
    this.PlayerStatus = LetterJamPlayerStatus.ReviewingGameEnd;
    this.GameEnd();
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
