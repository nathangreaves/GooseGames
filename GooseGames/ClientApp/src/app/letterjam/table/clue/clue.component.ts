import { Component, OnInit, Input, TemplateRef } from '@angular/core';
import { IPlayerSessionGame } from '../../../../models/session';
import { IGetPlayersFromCacheRequest, AllPlayersFromCacheRequest } from '../../../../models/letterjam/content';
import { IGooseGamesPlayer } from '../../../../models/player';
import { ILetterCard } from '../../../../models/letterjam/letters';
import { ICardsRequest } from '../../../../models/letterjam/table';
import { ClueLetter, IClue } from '../../../../models/letterjam/clues';
import { LetterJamCluesService } from '../../../../services/letterjam/clues';
import _ from 'lodash';

export interface ILetterJamClueComponentParameters {
  request: IPlayerSessionGame
  getPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  getCardsFromCache: (request: ICardsRequest) => Promise<ILetterCard[]>;
  clue: IClue;
  highlightColour: string;
}

@Component({
  selector: 'letterjam-clue',
  templateUrl: './clue.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './clue.component.scss']
})
export class LetterJamClueComponent implements OnInit {

  @Input() parameters: ILetterJamClueComponentParameters;
  @Input() buttonsTemplate: TemplateRef<any>;

  ErrorMessage: string;
  ShowAsPlayerId: string;
  ClueLetters: ClueLetter[] = [];

  constructor(private clueService: LetterJamCluesService) { }

  ngOnInit(): void {

    if (this.parameters.clue.letters && this.parameters.clue.letters.length) {
      this.ClueLetters = this.parameters.clue.letters;
    }
    else {
      this.clueService.GetLettersForClue(this.parameters.request, this.parameters.clue.id)
        .then(response => {
          if (response.success) {

            _.each(response.data, clueLetter => {

              var letter = <ClueLetter>{
                ...clueLetter,
                player: null,
                loadingPlayer: true
              };

              this.ClueLetters.push(letter);
            })

          }
          else {
            this.ErrorMessage = response.errorCode;
          }
          return response;
        })
        .then(response => {
          if (response.success) {
            return this.parameters.getPlayersFromCache(new AllPlayersFromCacheRequest(true, true))
              .then(players => {
                _.each(this.ClueLetters, cL => {

                  if (cL.playerId || cL.nonPlayerCharacterId) {
                    cL.player = _.find(players, p => p.id === (cL.playerId ?? cL.nonPlayerCharacterId));
                  }
                  cL.loadingPlayer = false;
                });
              });
          }
        }).then(() => {
          this.parameters.clue.letters = this.ClueLetters;
        })
    }

  }

  getLetter(letter: ClueLetter) {
    if (!letter.letter) {
      return letter.isWildCard ? "*" : "?";
    }
    return letter.letter;
  }

  show(playerId: string) {
    this.ShowAsPlayerId = playerId;
  }

  hide() {
    this.ShowAsPlayerId = null;
  }

  styleLetterCard = (letter: ClueLetter) => {
    if (letter.playerId == this.parameters.request.PlayerId && this.parameters.highlightColour) {
      var color = this.parameters.highlightColour;
      return {
        'border-color': color,
        'box-shadow': `1px 1px 2px 0px ${color}`
      }
    }
    return {};
  }
}
