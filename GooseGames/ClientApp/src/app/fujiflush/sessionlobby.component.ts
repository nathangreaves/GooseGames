import { Component } from '@angular/core';
import * as _ from 'lodash';
import { FujiPlayerDetailsService } from '../../services/fujiflush/playerdetails'
import { FujiSessionService } from '../../services/fujiflush/session'
import { GenericResponse } from '../../models/genericresponse'
import { PlayerDetails, UpdatePlayerDetailsRequest, PlayerDetailsResponse } from '../../models/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { IPlayerSessionComponent } from '../../models/session';

@Component({
  selector: 'app-fuji-sessionlobby-component',
  templateUrl: './sessionlobby.component.html',
  styleUrls: ['./sessionlobby.component.css'],
})

export class FujiSessionLobbyComponent implements IPlayerSessionComponent {

  private _playerDetailsService: FujiPlayerDetailsService;
  private _sessionService: FujiSessionService;
  private _router: Router;
  private _hubConnection: signalR.HubConnection;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;

  MinPlayers: number = 2;

  public Loading: boolean;
  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;
  public Players: PlayerDetailsResponse[];


  DisableButtons: boolean;

  constructor(playerDetailsService: FujiPlayerDetailsService, sessionService: FujiSessionService, router: Router, activatedRoute: ActivatedRoute) {
    this._playerDetailsService = playerDetailsService;
    this._sessionService = sessionService;
    this._router = router;

    this.Loading = true;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.setupConnection();

    this.Validate()
      .then(() => this.load());
  }

  private Validate(): Promise<any> {
    return Promise.resolve(null);
    //TODO: Some player validation here
    //return this._playerStatusService.Validate(this, PlayerStatus.InLobby, () => { this.CloseConnection(); });
  }

  public StartGame() {
    if (_.find(this.Players, p => p.playerNumber == 0)) {
      this.ErrorMessage = "Not all players are ready";
      return;
    }

    this.DisableButtons = true;
    this._sessionService.StartSession(this)
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(() => this.HandleGenericError())
      .finally(() => this.DisableButtons = false);
  }

  public KickPlayer(playerId: string) {
    if (playerId == this.PlayerId) {
      return;
    }
    this.DisableButtons = true;
    this._playerDetailsService.DeletePlayer({ sessionMasterId: this.PlayerId, playerToDeleteId: playerId })
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(() => this.HandleGenericError())
      .finally(() => this.DisableButtons = false);
  }

  private load() {
    this._playerDetailsService.GetPlayerDetails({ playerId: this.PlayerId, sessionId: this.SessionId })
      .then(data => {
        if (data.success) {

          this.SessionMaster = data.data.sessionMaster;
          this.SessionMasterName = data.data.sessionMasterName ? data.data.sessionMasterName : "Session Master";
          this.SessionMasterPlayerNumber = data.data.sessionMasterPlayerNumber ? data.data.sessionMasterPlayerNumber : null;
          this.Password = data.data.password;
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
      .catch(() => this.HandleGenericError());
  }
  private setupConnection() : Promise<any> {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/fujihub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {
      this.Validate();
    });
    this._hubConnection.onclose(() => {
      this.HandleGenericError();
    });

    this._hubConnection.on("playerAdded", (player: PlayerDetailsResponse) => {
      this.setDefaultNewPlayerName(player);
      this.Players.push(player);
    });
    this._hubConnection.on("playerDetailsUpdated", (player: PlayerDetailsResponse) => {
      if (this.Players && this.Players.length > 0) {
        var index = _.findIndex(this.Players, p => p.id == player.id);
        this.Players.splice(index, 1, player);
      }
    });
    this._hubConnection.on("playerRemoved", (playerId: string) => {
      if (this.Players && this.Players.length > 0) {
        _.remove(this.Players, p => p.id === playerId);
      }
    });
    this._hubConnection.on("startingSession", () => {
      this.CloseConnection();
      this._router.navigate(['/fujiflush/waiting', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    return this._hubConnection.start().catch(err => console.error(err));
  }

  CloseConnection() {
    var connection = this._hubConnection;
    if (connection) {
      connection.off("playerAdded");
      connection.off("playerDetailsUpdated");
      connection.off("playerRemoved");
      connection.off("startingSession");

      connection.onclose(() => { });

      connection.stop().then(() => {
        this._hubConnection = null;
      });
    }
  }

  private setDefaultNewPlayerName(player: PlayerDetailsResponse) {
    if (!player.playerName) {
      player.playerName = "New Player";
    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
