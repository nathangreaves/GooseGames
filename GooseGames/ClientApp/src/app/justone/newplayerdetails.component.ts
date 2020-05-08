import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../services/justone/playerdetails'
import { JustOnePlayerStatusService } from '../../services/justone/playerstatus'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../models/player'
import { IPlayerSessionComponent } from '../../models/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { PlayerStatus, PlayerStatusValidationResponse } from '../../models/justone/playerstatus';
import { JustOneLocalStorage } from '../../services/justone/localstorage';

@Component({
  selector: 'app-just-one-newplayerdetails-component',
  templateUrl: './newplayerdetails.component.html'
})

export class JustOneNewPlayerDetailsComponent implements IPlayerSessionComponent {

  private _playerDetailsService: JustOnePlayerDetailsService;
  private _playerStatusService: JustOnePlayerStatusService;
  private _justOneLocalStorage: JustOneLocalStorage;
  private _router: Router;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;

  PlayerName: string;

  constructor(playerDetailsService: JustOnePlayerDetailsService, playerStatusService: JustOnePlayerStatusService, justOneLocalStorage: JustOneLocalStorage,
    router: Router, activatedRoute: ActivatedRoute) {
    this._playerDetailsService = playerDetailsService;
    this._playerStatusService = playerStatusService;
    this._justOneLocalStorage = justOneLocalStorage;
    this._router = router;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.PlayerName = this._justOneLocalStorage.GetPlayerName();

    this._playerStatusService.Validate(this, PlayerStatus.New, () => { })
      .then(data => {
        if (data.success) {
          this.Loading = false;
        }
      });
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
          return this._playerStatusService.Set(this.PlayerId, PlayerStatus.InLobby);
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .then(data => {
        if (data.success) {

          this._justOneLocalStorage.CachePlayerDetails(this, this.PlayerName);
          this._router.navigate(['/justone/sessionlobby', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch(() => this.HandleGenericError());
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
