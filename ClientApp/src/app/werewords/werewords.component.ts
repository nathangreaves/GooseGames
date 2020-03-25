import { Component } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsGameService } from '../../services/WerewordsGameService'

@Component({
  selector: 'app-werewords-component',
  templateUrl: './werewords.component.html'
})

export class WerewordsComponent {

  public Games = [];
  public Error = null;

 private _werewordsGameService: WerewordsGameService;

  constructor(werewordsGameService: WerewordsGameService) {
    this._werewordsGameService = werewordsGameService;
  }


  public StartNewGame(password: string) {
    this.clearError();

    if (!password) {
      this.Error = "No password given for new game";
      return;
    }

    var foundGame = this.findGame(password);
    if (foundGame) {
      this.Error = "Game " + password +  " already exists";
      return;
    }

    this._werewordsGameService.CreateGame(password).then(data =>
    {
      this.Error = "Game added " + data;
      this.Games.push({ password: password, players: 1 });
    });

  }

  public JoinGame(password: string) {
    this.clearError();

    var foundGame = this.findGame(password);

    if (foundGame) {
      foundGame.players++;
    }
    else {
      this.Error = "Game not found with password " + password;
    }
  }

  private findGame(password: string) {
        return _.find(this.Games, function(game) { return game.password === password; });
    }

  clearError() {
    this.Error = null;
  }
}
