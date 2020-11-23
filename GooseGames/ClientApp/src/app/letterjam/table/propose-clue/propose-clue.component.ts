import { Component, OnInit, Input } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterCard } from '../../../../models/letterjam/letters';
import { AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import _ from 'lodash';
import { IGooseGamesPlayer } from '../../../../models/player';
import { LetterJamCluesService } from '../../../../services/letterjam/clues';

export interface IProposeClueComponentParameters extends ITableComponentParameters {
  proposedClues: () => void;
}

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
  ShowAsPlayerId: string = null;
  DisableButtons: boolean = false;
  WildCardId: string;

  constructor(private clueService: LetterJamCluesService) {
    super();
    this.WildCardId = clueService.WildCardId;
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
          cardId: this.WildCardId,
          letter: "*",
          nonPlayerCharacterId: null,
          playerId: this.WildCardId,
          loadingPlayer: false,
          player: {
            id: this.WildCardId,
            name: null,
            emoji: null,
            playerNumber: 100
          }
        })
      });
  }

  loadPlayers = (): Promise<any> => {
    return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true)).then(r => {

      _.each(this.RelevantLetters, l => {
        if (!l.bonusLetter && l.cardId !== this.WildCardId) {
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

  show(playerId: string) {
    this.ShowAsPlayerId = playerId;
  }

  hide() {
    this.ShowAsPlayerId = null;
  }

  DeleteLastLetter = () => {
    if (this.BuiltWord.length > 0) {
      this.BuiltWord = this.BuiltWord.slice(0, this.BuiltWord.length - 1);
    }
  }

  AddLetterToWord = (letter: LetterCard) => {
    this.BuiltWord.push(letter);
  }

  SubmitClue = () => {
    if (this.BuiltWord.length < 1) {
      this.ErrorMessage = "Please submit a word with at least 1 letter!"
    }
    this.DisableButtons = true;

    this.clueService.SubmitClue(this.parameters.request,
      this.parameters.getCurrentRoundId(),
      this.BuiltWord)
      .then(response => this.HandleGenericResponseBase(response, () => {
        this.parameters.proposedClues();
        this.BuiltWord = [];
        return response;
      }))
      .catch(this.HandleGenericError)
      .finally(() => {
        this.DisableButtons = false;
      });
  }

  Back = () => {
    this.parameters.proposedClues();
  }
}
