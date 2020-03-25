import { Component } from '@angular/core';
import * as _ from 'lodash';

@Component({
  selector: 'app-werewords-component',
  templateUrl: './werewords.component.html'
})

export class WerewordsComponent {

  public Games = [];
  public Error = null;

  public StartNewGame(password: string) {
    this.clearError();

    if (!password) {
      this.Error = "No password given for new game";
      return;
    }
    this.Games.push({ password: password, players: 1 });
  }

  public JoinGame(password: string) {
    this.clearError();

    var foundGame = _.find(this.Games, function (game) { return game.password === password; });

    if (foundGame) {
      foundGame.players++;
    }
    else {
      this.Error = "Game not found with password " + password;
    }
  }


  clearError() {
    this.Error = null;
  }
}
