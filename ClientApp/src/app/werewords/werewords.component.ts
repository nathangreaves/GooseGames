import { Component } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsGameService } from '../../services/WerewordsGameService'

@Component({
  selector: 'app-werewords-component',
  templateUrl: './werewords.component.html'
})

export class WerewordsComponent {

  public Games = [];
  public Message = null;

 private _werewordsGameService: WerewordsGameService;

  constructor(werewordsGameService: WerewordsGameService) {
    this._werewordsGameService = werewordsGameService;
  }


  public StartNewGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.Message = "No password given for new game";
      return;
    }

    var foundGame = this.findGame(password);
    if (foundGame) {
      this.Message = "Game " + password +  " already exists";
      return;
    }

    this._werewordsGameService.CreateGame(password).then(data =>
    {
      if (data.gameId) {
        this.Message = "Game added " + data.gameId;
        this.Games.push({ Id: data.gameId, Password: password, NumberOfPlayers: 1 });
      }
      else {
        this.Message = data.errorMessage;
      }
    });

  }

  public JoinGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.Message = "No password given for joining game";
      return;
    }

    this._werewordsGameService.JoinGame(password).then(data => {
      if (data.gameId) {
        this.Message = "Game joined " + data.gameId;

        var foundGame = this.findGame(password);

        if (foundGame && data.gameId === foundGame.Id) {
          foundGame.NumberOfPlayers = data.numberOfPlayers;
        }
        else {
          this.Games.push({ Id: data.gameId, Password: password, NumberOfPlayers: data.numberOfPlayers });
        }
      }
      else {
        this.Message = data.errorMessage;
      }
    });

  }

  private findGame(password: string) {
        return _.find(this.Games, function(game) { return game.Password === password; });
    }

  clearMessage() {
    this.Message = null;
  }
}
