import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../services/justone/playerdetails'
import { GenericResponse } from '../../models/genericresponse'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-just-one-newplayerdetails-component',
  templateUrl: './newplayerdetails.component.html'
})

export class JustOneNewPlayerDetailsComponent {

  private _playerDetailsService: JustOnePlayerDetailsService;
  private _router: Router;

  private _sessionId: string;
  private _playerId: string;

  public ErrorMessage: string;

  constructor(playerDetailsService: JustOnePlayerDetailsService, router: Router, activatedRoute: ActivatedRoute) {
    this._playerDetailsService = playerDetailsService;
    this._router = router;

    this._sessionId = activatedRoute.snapshot.params.sessionId;
    this._playerId = activatedRoute.snapshot.params.playerId;
  }

  public SubmitPlayerDetails(playerDetails: PlayerDetails) {

    var playerDetailsRequest = <UpdatePlayerDetailsRequest>playerDetails;
    playerDetailsRequest.sessionId = this._sessionId;
    playerDetailsRequest.playerId = this._playerId;

    this._playerDetailsService.UpdatePlayerDetails(playerDetailsRequest)
      .then(data => {
        if (data.success) {
          this._router.navigate(['/justone/sessionlobby', { sessionId: this._sessionId, playerId: this._playerId }])
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch(() => this.handleGenericError());
  }

  handleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
