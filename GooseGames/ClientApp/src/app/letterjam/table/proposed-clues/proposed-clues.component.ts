import { Component, OnInit, Input, OnDestroy, HostListener } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterJamCluesService } from '../../../../services/letterjam/clues';
import { AllPlayersFromCacheRequest, PlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { ProposedClue, ProposedClueVote, IProposedClue, RoundStatusEnum } from '../../../../models/letterjam/clues';
import _ from 'lodash';
import { LetterCard, ILetterCard } from '../../../../models/letterjam/letters';

import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ILetterJamClueComponentParameters } from '../clue/clue.component';

export interface IProposedCluesComponentParameters extends ITableComponentParameters {
  proposeClue: () => void;
}

@Component({
  selector: 'letterjam-proposed-clues',
  templateUrl: './proposed-clues.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './proposed-clues.component.scss']
})
export class LetterJamProposedCluesComponent extends TableComponentBase implements OnInit, OnDestroy {

  @Input() parameters: IProposedCluesComponentParameters;

  ProposedClues: ProposedClue[] = [];
  RelevantLetters: LetterCard[] = [];
  RoundStatus: RoundStatusEnum;

  clueModalParameters: ILetterJamClueComponentParameters;
  disableProposeClue: boolean;
  modalClueId: string;
  modalRef: NgbModalRef;

  constructor(private cluesService: LetterJamCluesService,
    private modalService: NgbModal) {
    super();
  }

  ngOnInit(): void {
    this.load();
    //this.parameters.hubConnection.on("tokenUpdate", this.processTokenUpdate);
    //this.parameters.hubConnection.on("bonusLetterGuessed", this.onBonusLetterGuessed);

  }


  onPlayerMovedOnToNextCard = (playerId: string, nextCard: ILetterCard) => {
    if (playerId !== this.parameters.request.PlayerId && nextCard) {
      var relevantCard = _.find(this.RelevantLetters, p => p.playerId === playerId);
      if (relevantCard) {

        relevantCard.bonusLetter = nextCard.bonusLetter;
        relevantCard.cardId = nextCard.cardId;
        relevantCard.letter = nextCard.letter;
      }
    }
  }
  onNewBonusCard = (newBonusCard: ILetterCard) => {
    if (newBonusCard.playerId !== this.parameters.request.PlayerId) {
      var relevantCard = _.find(this.RelevantLetters, p => p.playerId === newBonusCard.playerId);
      if (relevantCard) {

        relevantCard.bonusLetter = newBonusCard.bonusLetter;
        relevantCard.cardId = newBonusCard.cardId;
        relevantCard.letter = newBonusCard.letter;
      }
    }
  }
  onNewNpcCard = (newNpcCard: ILetterCard) => {
    var relevantCard = _.find(this.RelevantLetters, p => p.nonPlayerCharacterId === newNpcCard.nonPlayerCharacterId);
    if (relevantCard) {
      relevantCard.cardId = newNpcCard.cardId;
      relevantCard.letter = newNpcCard.letter;
    }
  }

  load = () => {
    this.cluesService.GetClues(this.parameters.request, this.parameters.getCurrentRoundId())
      .then(response => this.parameters.handleGenericResponse(response, r => {

        this.RoundStatus = r.roundStatus;

        _.each(r.clues, proposedClue => {
          var clue = <ProposedClue>{
            ...proposedClue,
            loadingPlayer: true,
            player: null,
            votes: [],
            letters: []
          };

          var iVoted = false;

          _.each(proposedClue.votes, v => {
            clue.votes.push({
              ...v,
              player: null,
              loadingPlayer: true
            });
            if (v.playerId === this.parameters.request.PlayerId) {
              iVoted = true;
              this.disableProposeClue = true;
            }
          });

          clue.voted = iVoted;
          clue.myClue = clue.playerId === this.parameters.request.PlayerId;
          this.ProposedClues.push(clue);
        });
        return response;
      }))
      .then(response => this.parameters.handleGenericResponseBase(response, () => {
        this.subscribe();
        return this.loadRelevantLetters().then(() => this.loadPlayers()).then(() => response);
      }));
  }

  onAddClue = (proposedClue: IProposedClue) => {
    var clue = <ProposedClue>{
      ...proposedClue,
      loadingPlayer: true,
      player: null,
      votes: [],
      letters: []
    };
    clue.myClue = clue.playerId === this.parameters.request.PlayerId;

    this.ProposedClues.push(clue);

    this.parameters.getPlayersFromCache(new PlayersFromCacheRequest([clue.playerId], true, false))
      .then(player => {
        clue.player = player[0];
        clue.loadingPlayer = false;
      });
  }

  onRemoveClue = (clueId: string) => {
    var indexOf = _.findIndex(this.ProposedClues, c => {
      return c.id === clueId;
    });
    if (indexOf >= 0) {
      this.ProposedClues.splice(indexOf, 1);
    }
  }

  onAddVote = (playerId: string, clueId: string) => {
    this.parameters.getPlayersFromCache(new PlayersFromCacheRequest([playerId], true, false))
      .then(player => {
        var clue = _.find(this.ProposedClues, clue => clue.id === clueId);
        if (clue) {
          clue.votes.push(<ProposedClueVote>{
            id: '',
            loadingPlayer: false,
            player: player[0],
            playerId: playerId
          });
          this.disableProposeClue = true;
        }
      });
  };

  onRemoveVote = (playerId: string, clueId: string) => {
    var clue = _.find(this.ProposedClues, clue => clue.id === clueId);
    if (clue) {
      var voteIndex = _.findIndex(clue.votes, v => v.playerId === playerId);
      if (voteIndex >= 0) {
        clue.votes.splice(voteIndex, 1);
        this.disableProposeClue = false;
        if (playerId === this.parameters.request.PlayerId) {
          clue.voted = false;
        }
      }
    }
  };

  subscribe() {
    this.parameters.hubConnection.on("addVote", this.onAddVote);
    this.parameters.hubConnection.on("removeVote", this.onRemoveVote);
    this.parameters.hubConnection.on("newClue", this.onAddClue);
    this.parameters.hubConnection.on("removeClue", this.onRemoveClue);
    this.parameters.hubConnection.on("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.on("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.on("newNpcCard", this.onNewNpcCard);
    this.parameters.hubConnection.on('giveClue', this.onGiveClue);
    this.parameters.hubConnection.on('beginNewRound', this.onBeginNewRound);
    this.parameters.hubConnection.on('removeBonusCard', this.onRemoveBonusCard);
  }

  ngOnDestroy() {
    this.parameters.hubConnection.off("addVote", this.onAddVote);
    this.parameters.hubConnection.off("removeVote", this.onRemoveVote);
    this.parameters.hubConnection.off("newClue", this.onAddClue);
    this.parameters.hubConnection.off("removeClue", this.onRemoveClue);
    this.parameters.hubConnection.off("playerMovedOnToNextCard", this.onPlayerMovedOnToNextCard);
    this.parameters.hubConnection.off("newBonusCard", this.onNewBonusCard);
    this.parameters.hubConnection.off("newNpcCard", this.onNewNpcCard);
    this.parameters.hubConnection.off("giveClue", this.onGiveClue);
    this.parameters.hubConnection.off('beginNewRound', this.onBeginNewRound);
    this.parameters.hubConnection.off('removeBonusCard', this.onRemoveBonusCard);
  }
  onGiveClue = () => {
    this.ProposedClues = [];
  }
  onBeginNewRound = () => {
    this.ProposedClues = [];
  }

  onRemoveBonusCard = (cardId: string) => {
    var index = _.findIndex(this.RelevantLetters, b => b.cardId === cardId);
    if (index >= 0) {
      this.RelevantLetters.splice(index, 1);
    }
  }

  loadRelevantLetters = (): Promise<any> => {
    return this.parameters.getCardsFromCache({ relevantCards: true, cardIds: null })
      .then(r => {
        _.each(r, letterCard => {
          this.RelevantLetters.push({
            ...letterCard,
            loadingPlayer: true,
            player: null
          });
        });
      });
  }

  loadPlayers = (): Promise<any> => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true)).then(r => {
      _.each(this.ProposedClues, p => {
        p.player = _.find(r, fP => fP.id == p.playerId);
        p.loadingPlayer = false;

        _.each(p.votes, v => {
          v.player = _.find(r, fP => fP.id == v.playerId);
          v.loadingPlayer = false;
        })
      });

      _.each(this.RelevantLetters, l => {
        if (!(l.bonusLetter && !l.playerId)) {
          l.player = _.find(r, fP => fP.id == l.playerId || fP.id == l.nonPlayerCharacterId);
        }
        l.loadingPlayer = false;
      });

      this.RelevantLetters = _.sortBy(this.RelevantLetters, l => l.playerId ? l.player.playerNumber : l.nonPlayerCharacterId ? l.player.playerNumber + 10 : 20);
    });
  }

  ProposeClue = () => {
    this.parameters.proposeClue();
  }

  Vote(clue: ProposedClue) {

    if (clue.voted) {
      clue.voted = false;

      this.cluesService.Vote(this.parameters.request, this.parameters.getCurrentRoundId(), null);
    }
    else {
      _.each(this.ProposedClues, c => {
        c.voted = false;
      });

      clue.voted = true;

      this.cluesService.Vote(this.parameters.request, this.parameters.getCurrentRoundId(), clue.id);
    }
  }

  DeleteClue = () => {
    var clueId = this.modalClueId;

    this.modalRef.dismiss();
    this.cluesService.DeleteClue(this.parameters.request, clueId)
      .then(response => this.parameters.handleGenericResponseBase(response, () => response));
  }

  CloseClueModal = () => {
    this.modalRef.dismiss();
  }

  OpenClue(e: Event, clue: ProposedClue, content: any) {

    if (clue.playerId === this.parameters.request.PlayerId) {
      e.stopPropagation();

      this.modalClueId = clue.id;
      this.clueModalParameters = <ILetterJamClueComponentParameters>{
        request: this.parameters.request,
        getCardsFromCache: this.parameters.getCardsFromCache,
        getPlayersFromCache: this.parameters.getPlayersFromCache,
        clue: clue,
        highlightColour: null,
        showEmojis: true
      };

      const modalState = {
        modal: true,
        desc: 'fake state for our modal'
      };
      history.pushState(modalState, null);
      var modalRef = this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
      this.modalRef = modalRef;

      modalRef.result.finally(() => {
        this.clueModalParameters = null;
        this.modalRef = null;
        this.onHidden();
        this.modalClueId = null;
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

  onHidden = () => {
    if (window.history.state.modal === true) {
      window.history.state.modal = false;
      history.back();
    }
  }
}
