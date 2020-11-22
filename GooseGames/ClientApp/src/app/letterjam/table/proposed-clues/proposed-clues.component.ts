import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterJamCluesService } from '../../../../services/letterjam/clues';
import { AllPlayersFromCacheRequest, PlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { ProposedClue, ProposedClueVote } from '../../../../models/letterjam/clues';
import _ from 'lodash';
import { ILetterCard, LetterCard } from '../../../../models/letterjam/letters';

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

  constructor(private cluesService: LetterJamCluesService) {
    super();
  }

  ngOnInit(): void {
    this.load();
  }

  load = () => {
    this.cluesService.GetClues(this.parameters.request, this.parameters.getCurrentRoundId())
      .then(response => this.HandleGenericResponse(response, r => {
        _.each(r, proposedClue => {
          var clue = <ProposedClue>{
            ...proposedClue,
            loadingPlayer: true,
            player: null,
            votes: []
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
            }
          });

          clue.voted = iVoted;
          this.ProposedClues.push(clue);
        });
        return response;
      }))
      .then(response => this.HandleGenericResponseBase(response, () => {
        this.subscribe();
        return this.loadRelevantLetters().then(() => this.loadPlayers()).then(() => response);
      }));
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
        }
      })

  };

  onRemoveVote = (playerId: string, clueId: string) => {
    var clue = _.find(this.ProposedClues, clue => clue.id === clueId);
    if (clue) {
      var voteIndex = _.findIndex(clue.votes, v => v.playerId === playerId);
      clue.votes.splice(voteIndex, 1);
    }
  };

  subscribe() {
    this.parameters.hubConnection.on("addVote", this.onAddVote);
    this.parameters.hubConnection.on("removeVote", this.onRemoveVote);
  }

  ngOnDestroy() {
    this.parameters.hubConnection.off("addVote", this.onAddVote);
    this.parameters.hubConnection.off("removeVote", this.onRemoveVote);
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
        if (!l.bonusLetter) {
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
}
