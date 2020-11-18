import { Component, OnInit, Input } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterCard } from '../../../../models/letterjam/letters';
import { AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import _ from 'lodash';
import {
  trigger,
  state,
  style,
  animate,
  transition,
  keyframes
} from '@angular/animations';
import { IGooseGamesPlayer } from '../../../../models/player';
import { request } from 'https';

export interface IProposeClueComponentParameters extends ITableComponentParameters {
  proposedClues: () => void;
}

const WildCardId = "dd4750cc-07cf-497e-867d-6f434938677e";


//animations: [
//  trigger('caretAnimation', [
//    state('*', style({
//      opacity: 0
//    })),
//    transition('* <=> *', animate(5000, keyframes([
//      style({ opacity: 0, offset: 0 }),
//      style({ opacity: 1, offset: 0.2 }),
//      style({ opacity: 0, offset: 0.5 })])
//    ))],
//  )]  

@Component({
  selector: 'letterjam-propose-clue',
  templateUrl: './propose-clue.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './propose-clue.component.scss'] 
})
export class LetterJamProposeClueComponent extends TableComponentBase implements OnInit {
  @Input() parameters: IProposeClueComponentParameters;

  BuiltWord: LetterCard[] = [];
  RelevantLetters: LetterCard[] = [];
  HumanPlayers: IGooseGamesPlayer[] = [];

  constructor() {
    super();
  }

  ngOnInit(): void {
    this.loadRelevantLetters().then(this.loadPlayers).then(this.loadHumanPlayers);
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

        this.RelevantLetters.push({
          bonusLetter: false,
          cardId: WildCardId,
          letter: "*",
          nonPlayerCharacterId: null,
          playerId: null,
          loadingPlayer: false,
          player: null
        })
      });
  }

  loadPlayers = (): Promise<any> => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true)).then(r => {

      _.each(this.RelevantLetters, l => {
        if (!l.bonusLetter) {
          l.player = _.find(r, fP => fP.id == l.playerId || fP.id == l.nonPlayerCharacterId);
        }
        l.loadingPlayer = false;
      });

      this.RelevantLetters = _.sortBy(this.RelevantLetters, l => l.playerId ? l.player.playerNumber : l.nonPlayerCharacterId ? l.player.playerNumber + 10 : 20);
    });
  }

  loadHumanPlayers = (): Promise<any> => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, false)).then(r => {

      _.each(r, p => {
        if (p.id !== this.parameters.request.PlayerId)
          this.HumanPlayers.push(p);
      });

    });
  }

  DeleteLastLetter = () => {
    if (this.BuiltWord.length > 0) {
      this.BuiltWord = this.BuiltWord.slice(0, this.BuiltWord.length - 1);
    }
  }

  AddLetterToWord = (letter: LetterCard) => {
    this.BuiltWord.push(letter);
  }
}
