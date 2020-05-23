import { Component } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsComponentBase, WerewordsPlayerStatus } from '../../../models/werewords/content';
import { UpdatePlayerDetailsRequest } from '../../../models/player';
import { GooseGamesLocalStorage } from '../../../services/localstorage';
import { WerewordsSessionService } from '../../../services/werewords/session';

@Component({
  selector: 'app-werewords-newplayer-component',
  templateUrl: './newplayer.html'
})

export class WerewordsNewPlayerDetailsComponent extends WerewordsComponentBase {
    
  PlayerName: string;

  constructor(private _playerDetailsService: WerewordsSessionService, private _localStorage: GooseGamesLocalStorage) {

    super();

    
    this.PlayerName = this._localStorage.GetPlayerName();

    this.Loading = false;
  }

  public SubmitPlayerDetails() {

    var playerDetailsRequest = <UpdatePlayerDetailsRequest>
      {
        playerName: this.PlayerName,
        sessionId: this.SessionId,
        playerId: this.PlayerId
      };

    this._playerDetailsService.UpdatePlayerDetails(playerDetailsRequest)
      .then(data => {
        if (data.success) {

          this._localStorage.CachePlayerName(this.PlayerName);
          this.Route(WerewordsPlayerStatus.InLobby);
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch(err => this.HandleGenericError(err));
  }
}
