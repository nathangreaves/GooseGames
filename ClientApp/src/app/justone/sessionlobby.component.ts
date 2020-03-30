import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../services/justone/playerdetails'
import { JustOneSessionService } from '../../services/justone/session'
import { GenericResponse } from '../../models/genericresponse'
import { PlayerDetails, UpdatePlayerDetailsRequest, PlayerDetailsResponse } from '../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { JustOnePlayerStatusService } from '../../services/justone/playerstatus'
import { PlayerStatus } from '../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../models/justone/session';

@Component({
  selector: 'app-just-one-sessionlobby-component',
  templateUrl: './sessionlobby.component.html',
  styleUrls: ['./sessionlobby.component.css'],
})

export class JustOneSessionLobbyComponent implements IPlayerSessionComponent {

  private _playerDetailsService: JustOnePlayerDetailsService;
  private _sessionService: JustOneSessionService;
  private _playerStatusService: JustOnePlayerStatusService;
  private _router: Router;
  private _hubConnection: signalR.HubConnection;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;

  public Loading: boolean;
  public SessionMaster: boolean;
  public SessionMasterName: string;
  public Players: PlayerDetailsResponse[];

  constructor(playerDetailsService: JustOnePlayerDetailsService, sessionService: JustOneSessionService, playerStatusService: JustOnePlayerStatusService, router: Router, activatedRoute: ActivatedRoute) {
    this._playerDetailsService = playerDetailsService;
    this._playerStatusService = playerStatusService;
    this._sessionService = sessionService;
    this._router = router;

    this.Loading = true;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;


    this._playerStatusService.Validate(this, PlayerStatus.InLobby, () => { })
      .then(() => this.load());
  }

  public StartGame()
  {
    this._sessionService.StartSession(this)
      .catch(() => this.HandleGenericError());
  }

  public KickPlayer(playerId: string) {
    if (playerId == this.PlayerId) {
      return;
    }
    this._playerDetailsService.DeletePlayer({ sessionMasterId: this.PlayerId, playerToDeleteId: playerId })
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(() => this.HandleGenericError());
  }

  private load() {
    this._playerDetailsService.GetPlayerDetails({ playerId: this.PlayerId, sessionId: this.SessionId })
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
      .catch(() => this.HandleGenericError());
  }
  private setupConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();

    this._hubConnection.on("playerAdded", (player: PlayerDetailsResponse) => {
      this.setDefaultNewPlayerName(player);
      this.Players.push(player);
    });
    this._hubConnection.on("playerDetailsUpdated", (player: PlayerDetailsResponse) => {
      var index = _.findIndex(this.Players, p => p.id == player.id);
      this.Players.splice(index, 1, player);
    });
    this._hubConnection.on("playerRemoved", (playerId: string) => {
      _.remove(this.Players, p => p.id === playerId);
    });
    this._hubConnection.on("startingSession", () => {
      //this._hubConnection.stop();
      //this._hubConnection = null;;
      this._router.navigate(['/justone/round/waiting', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.start().catch(err => console.error(err));
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
