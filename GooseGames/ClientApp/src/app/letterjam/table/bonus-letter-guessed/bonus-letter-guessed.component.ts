import { Component, OnInit, Input } from '@angular/core';
import { IBonusLetterGuess } from '../../../../models/letterjam/letters';
import { IGooseGamesPlayer } from '../../../../models/player';
import { IPlayerSessionGame } from '../../../../models/session';

export interface ILetterJamBonusLetterGuessedParameters extends IBonusLetterGuess {
  player: IGooseGamesPlayer
  sessionInfo: IPlayerSessionGame
}

@Component({
  selector: 'letterjam-bonus-letter-guessed',
  templateUrl: './bonus-letter-guessed.component.html',
  styleUrls: ['../../common/letterjam.common.scss',
    './bonus-letter-guessed.component.scss']
})
export class LetterJamBonusLetterGuessedComponent implements OnInit {

  @Input() parameters: ILetterJamBonusLetterGuessedParameters;

  constructor() { }

  ngOnInit(): void {
  }

  PlayerName = () => {
    if (this.parameters.sessionInfo.PlayerId === this.parameters.playerId) {
      return "You";
    }
    return this.parameters.player.name;
  }
}
