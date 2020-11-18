import { Component, OnInit, Input } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterJamCluesService } from '../../../../services/letterjam/clues';
import { AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { ProposedClue } from '../../../../models/letterjam/clues';
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
export class LetterJamProposedCluesComponent extends TableComponentBase implements OnInit {
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
          _.each(proposedClue.votes, v => {
            clue.votes.push({
              ...v,
              player: null,
              loadingPlayer: true
            })
          });
          this.ProposedClues.push(clue);
        });
        return response;
      }))
      .then(response => this.HandleGenericResponseBase(response, () => {
        return this.loadRelevantLetters().then(() => this.loadPlayers()).then(() => response);
      }));
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
}
