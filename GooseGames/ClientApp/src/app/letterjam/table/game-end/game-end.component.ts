import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { TableComponentBase, ITableComponentParameters } from '../table-base.component';
import { LetterJamGameEndService } from '../../../../services/letterjam/gameEnd';
import { IGameEndPlayer, GameEndPlayer } from '../../../../models/letterjam/gameEnd';
import { PlayersFromCacheRequest, AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import _ from 'lodash';
import { ILetterJamClueComponentParameters } from '../clue/clue.component';

export interface ILetterJamGameEndComponentParameters extends ITableComponentParameters {

}

@Component({
  selector: 'letterjam-game-end',
  templateUrl: './game-end.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './game-end.component.scss']
})
export class LetterJamGameEndComponent extends TableComponentBase implements OnInit, OnDestroy {

  @Input() parameters: ILetterJamGameEndComponentParameters;

  Players: GameEndPlayer[];

  constructor(private gameEndService: LetterJamGameEndService) {
    super();
  }

  ngOnInit(): void {
    this.gameEndService.GetGameEnd(this.parameters.request)
      .then(response => this.parameters.handleGenericResponse(response, r => {

        this.Players = r.players.map(p => {
          return {
            ...p,
            loadingPlayer: true,
            player: null
          };
        });

        return response;
      }))
      .then(() => this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, false)))
      .then(players => {
        _.each(this.Players, player => {
          player.player = _.find(players, p => p.id === player.playerId);
          player.loadingPlayer = false;
        })
      })
      .finally(() => {
        this.Loading = false;
      });
  }
  ngOnDestroy(): void {
  }

  LetterJamActualLetterClueParameters(player: GameEndPlayer) {
    return <ILetterJamClueComponentParameters>{
      clue: {
        id: null,
        letters: player.finalWordLetters.map(m => {
          return {
            bonusLetter: m.bonusLetter,
            cardId: m.cardId,
            isWildCard: m.isWildCard,
            letter: m.letter,
            loadingPlayer: false,
            player: player.player,
            nonPlayerCharacterId: null,
            playerId: player.playerId
          }
        })
      },
      showEmojis: false,
      getCardsFromCache: null,
      getPlayersFromCache: null,
      highlightColour: null,
      request: this.parameters.request
    }
  }


  LetterJamGuessedLetterClueParameters(player: GameEndPlayer) {
    return <ILetterJamClueComponentParameters>{
      clue: {
        id: null,
        letters: player.finalWordLetters.map(m => {
          return {
            bonusLetter: m.bonusLetter,
            cardId: m.cardId,
            isWildCard: m.isWildCard,
            letter: m.playerLetterGuess,
            loadingPlayer: false,
            player: player.player,
            nonPlayerCharacterId: null,
            playerId: player.playerId
          }
        })
      },
      showEmojis: false,
      getCardsFromCache: null,
      getPlayersFromCache: null,
      highlightColour: null,
      request: this.parameters.request
    }
  }

  LetterJamUnusedLetterClueParameters(player: GameEndPlayer) {
    return <ILetterJamClueComponentParameters>{
      clue: {
        id: null,
        letters: player.unusedLetters.map(m => {
          return {
            bonusLetter: m.bonusLetter,
            cardId: m.cardId,
            isWildCard: m.isWildCard,
            letter: m.letter,
            loadingPlayer: false,
            player: player.player,
            nonPlayerCharacterId: null,
            playerId: player.playerId
          }
        })
      },
      showEmojis: false,
      getCardsFromCache: null,
      getPlayersFromCache: null,
      highlightColour: null,
      request: this.parameters.request
    }
  }
}

