import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../services/justone/playerdetails'
import { GenericResponse } from '../../models/genericresponse'
import { PlayerDetails, UpdatePlayerDetailsRequest, PlayerDetailsResponse } from '../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-just-one-sessionlobby-component',
  templateUrl: './sessionlobby.component.html',
  styleUrls: ['./sessionlobby.component.css'],
})

export class JustOneSessionLobbyComponent {

  private _playerDetailsService: JustOnePlayerDetailsService;
  private _router: Router;

  private _sessionId: string;
  private _playerId: string;

  public ErrorMessage: string;
  public Loading: boolean;
  public SessionMaster: boolean;
  public SessionMasterName: string;
  public Players: PlayerDetailsResponse[];

  constructor(playerDetailsService: JustOnePlayerDetailsService, router: Router, activatedRoute: ActivatedRoute) {
    this._playerDetailsService = playerDetailsService;
    this._router = router;

    this.Loading = true;

    this._sessionId = activatedRoute.snapshot.params.sessionId;
    this._playerId = activatedRoute.snapshot.params.playerId;

    this.load();
  }

  private load() {
    this._playerDetailsService.GetPlayerDetails({ playerId: this._playerId, sessionId: this._sessionId })
      .then(data => {
        if (data.success) {
          this.SessionMaster = data.data.sessionMaster;
          this.SessionMasterName = data.data.sessionMasterName ? data.data.sessionMasterName : "Session Master";
          this.Players = data.data.players;
          _.forEach(this.Players, player =>
          {
            player.playerName = player.playerName ? player.playerName : "New Player";
          });
          this.Loading = false;
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch(() => this.handleGenericError());
  }

  //public SubmitPlayerDetails(playerDetails: PlayerDetails) {

  //  var playerDetailsRequest = <UpdatePlayerDetailsRequest>playerDetails;
  //  playerDetailsRequest.sessionId = this._sessionId;
  //  playerDetailsRequest.playerId = this._playerId;

  //  this._playerDetailsService.UpdatePlayerDetails(playerDetailsRequest)
  //    .then(data => {
  //      if (data.success) {
  //        this._router.navigate(['/justone/sessionlobby', { sessionId: this._sessionId, playerId: this._playerId }])
  //      }
  //      else {
  //        this.ErrorMessage = data.errorCode;
  //      }
  //    })
  //    .catch(() => this.handleGenericError());
  //}

  handleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
