import { Component, OnInit, Input } from '@angular/core';
import { IProposedCluesComponentParameters } from '../proposed-clues.component';
import { IPlayerSessionGame } from '../../../../../models/session';
import { IGetPlayersFromCacheRequest, AllPlayersFromCacheRequest } from '../../../../../models/letterjam/content';
import { IGooseGamesPlayer } from '../../../../../models/player';
import { ILetterCard } from '../../../../../models/letterjam/letters';
import { ICardsRequest } from '../../../../../models/letterjam/table';
import { ProposedClue, ClueLetter } from '../../../../../models/letterjam/clues';
import { LetterJamCluesService } from '../../../../../services/letterjam/clues';
import { LetterJamLetterCardService } from '../../../../../services/letterjam/letterCard';
import _ from 'lodash';

export interface ILetterJamProposedClueModalComponentParameters {
  onDeleteClue: (clueId: string) => void;
  onBack: () => void;
  request: IPlayerSessionGame
  getPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  getCardsFromCache: (request: ICardsRequest) => Promise<ILetterCard[]>;
  clue: ProposedClue;
}

@Component({
  selector: 'letterjam-clue-modal',
  templateUrl: './clue-modal.component.html',
  styleUrls: ['../../../common/letterjam.common.scss',
    './clue-modal.component.scss']
})
export class LetterJamProposedClueModalComponent implements OnInit {

  @Input() parameters: ILetterJamProposedClueModalComponentParameters;

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

  show(playerId: string) {
    this.ShowAsPlayerId = playerId;
  }

  hide() {
    this.ShowAsPlayerId = null;
  }


  Delete = () => {
    this.parameters.onDeleteClue(this.parameters.clue.id);
  }

  Back = () => {
    this.parameters.onBack();
  }

}
