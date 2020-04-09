import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../services/justone/playerdetails'
import { JustOnePlayerStatusService } from '../../services/justone/playerstatus'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../models/justone/player'
import { IPlayerSessionComponent } from '../../models/justone/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { PlayerStatus, PlayerStatusValidationResponse } from '../../models/justone/playerstatus';

@Component({
  selector: 'app-just-one-newplayerdetails-component',
  templateUrl: './newplayerdetails.component.html'
})

export class JustOneNewPlayerDetailsComponent implements IPlayerSessionComponent {

  private _playerDetailsService: JustOnePlayerDetailsService;
  private _playerStatusService: JustOnePlayerStatusService;
  private _router: Router;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;

  constructor(playerDetailsService: JustOnePlayerDetailsService, playerStatusService: JustOnePlayerStatusService, router: Router, activatedRoute: ActivatedRoute) {
    this._playerDetailsService = playerDetailsService;
    this._playerStatusService = playerStatusService;
    this._router = router;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this._playerStatusService.Validate(this, PlayerStatus.New, () => { })
      .then(data => {
        if (data.success) {
          this.Loading = false;
        }
      });
  }

  public SubmitPlayerDetails(playerDetails: PlayerDetails) {

    var playerDetailsRequest = <UpdatePlayerDetailsRequest>playerDetails;
    playerDetailsRequest.sessionId = this.SessionId;
    playerDetailsRequest.playerId = this.PlayerId;

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
