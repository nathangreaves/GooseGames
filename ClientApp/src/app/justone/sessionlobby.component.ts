import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../services/justone/playerdetails'
import { GenericResponse } from '../../models/genericresponse'
import { PlayerDetails, UpdatePlayerDetailsRequest, PlayerDetailsResponse } from '../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";

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
          this.setupConnection();

          this.SessionMaster = data.data.sessionMaster;
          this.SessionMasterName = data.data.sessionMasterName ? data.data.sessionMasterName : "Session Master";
          this.Players = data.data.players;
          _.forEach(this.Players, player => {
            this.setDefaultNewPlayerName(player);
          });
          this.Loading = false;
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch(() => this.handleGenericError());
  }
  private setupConnection() {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?playerId=${this._playerId}&sessionId=${this._sessionId}`)
      .build();

    connection.on("playerAdded", (player: PlayerDetailsResponse) => {
      this.setDefaultNewPlayerName(player);
      this.Players.push(player);
    });
    connection.on("playerDetailsUpdated", (player: PlayerDetailsResponse) => {
      var index = _.findIndex(this.Players, p => p.id == player.id);
      this.Players.splice(index, 1, player);
    });
    connection.on("playerRemoved", (player: PlayerDetailsResponse) => {
      _.remove(this.Players, p => p.id === player.id);
    });
    connection.start().catch(err => console.error(err));
  }

  private setDefaultNewPlayerName(player: PlayerDetailsResponse) {
    if (!player.playerName) {
      player.playerName = "New Player";
    }
  }

  private handleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
