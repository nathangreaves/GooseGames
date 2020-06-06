import { Component, OnDestroy, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { PlayerDetails, UpdatePlayerDetailsRequest, PlayerDetailsResponse } from '../../../models/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { WerewordsComponentBase, WerewordsPlayerStatus } from '../../../models/werewords/content';
import { WerewordsSessionService } from '../../../services/werewords/session';

@Component({
  selector: 'app-werewords-lobby-component',
  templateUrl: './lobby.html',
  styleUrls: ['./lobby.css'],
})
export class WerewordsLobbyComponent extends WerewordsComponentBase implements OnInit, OnDestroy {

  MinPlayers: number = 4;

  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;
  public Players: PlayerDetailsResponse[];


  DisableButtons: boolean;
    StatusText: string;
    _globalHubConnection: signalR.HubConnection;

  constructor(private _sessionService: WerewordsSessionService, private _router: Router, activatedRoute: ActivatedRoute) {
    super();

    this.Loading = true;

  }
  ngOnInit(): void {

    this.HubConnection.on("startingSession", () => {
      this.StatusText = "Setting up the game...";
    });
    this.HubConnection.on("secretRole", () => {
      
      this.Route(WerewordsPlayerStatus.NightRevealSecretRole);
    });

    this.setupConnection().then(() => {

      this._globalHubConnection.on("playerAdded", (player: PlayerDetailsResponse) => {
        this.setDefaultNewPlayerName(player);
        this.Players.push(player);
      });
      this._globalHubConnection.on("playerDetailsUpdated", (player: PlayerDetailsResponse) => {
        if (this.Players && this.Players.length > 0) {
          var index = _.findIndex(this.Players, p => p.id == player.id);
          this.Players.splice(index, 1, player);
        }
      });
      this._globalHubConnection.on("playerRemoved", (playerId: string) => {
        if (this.Players && this.Players.length > 0) {
          _.remove(this.Players, p => p.id === playerId);
        }

        if (playerId == this.PlayerId) {
          this.SetSessionData(null, null, null);
          this._router.navigate(['/werewords'], { replaceUrl: true });
        }
      });

      this.load();
    });
  }

  private setupConnection(): Promise<any> {

    if (this._globalHubConnection) {
      var oldConnection = this._globalHubConnection;
      oldConnection.onclose(() => { });
      oldConnection.stop();
    }

    this._globalHubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/globalhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._globalHubConnection.onreconnected(() => {
      //this.Validate();
    });
    this._globalHubConnection.onclose(() => {
      this.HandleGenericError("connection closed");
    });

    return this._globalHubConnection.start().catch(err => console.error(err));
  }

  ngOnDestroy(): void {

    this.CloseConnection();

  }


  CloseConnection() {
    var connection = this.HubConnection;
    if (connection) {
      connection.off("startingSession");
      connection.off("secretRole");
    }

    var globalConnection = this._globalHubConnection;
    if (globalConnection) {
      globalConnection.off("playerAdded");
      globalConnection.off("playerDetailsUpdated");
      globalConnection.off("playerRemoved");
    }
  }

  public StartGame() {

    this.ErrorMessage = null;
    if (_.find(this.Players, p => p.playerNumber == 0 || !p.ready)) {
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
      .catch((err) => {
        this.HandleGenericError(err);
        this.DisableButtons = false;
      });
  }

  public KickPlayer(playerId: string) {
    if (playerId == this.PlayerId) {
      return;
    }
    this.DisableButtons = true;
    this._sessionService.DeletePlayer({ sessionMasterId: this.PlayerId, playerToDeleteId: playerId })
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch((err) => this.HandleGenericError(err))
      .finally(() => this.DisableButtons = false);
  }

  private load() {
    this._sessionService.GetPlayerDetails(this)
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
      .catch((err) => this.HandleGenericError(err));
  }


  private setDefaultNewPlayerName(player: PlayerDetailsResponse) {
    if (!player.playerName) {
      player.playerName = "New Player";
    }
  }
}
