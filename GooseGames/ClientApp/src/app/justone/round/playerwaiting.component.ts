import * as _ from 'lodash';
import { Component, Input, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus, PlayerStatusValidationResponse } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { PlayerAction } from '../../../models/justone/playeractions';
import { GenericResponse } from '../../../models/genericresponse';
import { PlayerNumberCss } from '../../../services/justone/ui'
import * as signalR from '@microsoft/signalr';
import { connect } from 'tls';

export interface IJustOnePlayerWaitingComponent extends IPlayerSessionComponent {

  GetPlayerStatus(): PlayerStatus;
  LoadPlayers(): Promise<GenericResponse<PlayerAction[]>>;
  LoadContent(): Promise<any>;
  OnRedirect();

  CreateHubConnection(): signalR.HubConnectionBuilder;
  OnCloseHubConnection(connection: signalR.HubConnection);
  HubConnectionFailed();
  SetupHubConnection(connection: signalR.HubConnection);

  SetPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent);

  Loading: boolean;
}

export abstract class JustOnePlayerWaitingComponentBase implements IJustOnePlayerWaitingComponent {

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;

  abstract GetPlayerStatus(): PlayerStatus;
  abstract LoadPlayers(): Promise<GenericResponse<PlayerAction[]>>;
  abstract LoadContent(): Promise<any>;
  abstract SetPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent);

  abstract OnCloseHubConnection(connection: signalR.HubConnection);
  abstract SetupHubConnection(connection: signalR.HubConnection);

  constructor(activatedRoute: ActivatedRoute) {
    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;
  }

  OnRedirect() {

  }

  CreateHubConnection(): signalR.HubConnectionBuilder {
    var hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`);

    return hubConnection;
  }

  HubConnectionFailed() {
    this.HandleGenericError();
  }

  HandleGenericError() {
    this.ErrorMessage = "An unknown error occurred";
  }
}

@Component({
  selector: 'app-just-one-playerwaiting-component',
  templateUrl: './playerwaiting.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePlayerWaitingComponent implements OnInit {
  
  PlayerNumberCss = PlayerNumberCss;

  private _playerStatusService: JustOnePlayerStatusService;

  Players: PlayerAction[];
  @Input() playerWaitingComponent: IJustOnePlayerWaitingComponent;
  _connection: signalR.HubConnection;

  ngOnInit() {
    this.playerWaitingComponent.SetPlayerWaitingComponent(this);

    var connectionBuilder = this.playerWaitingComponent.CreateHubConnection();
    connectionBuilder.withAutomaticReconnect();
    this._connection = connectionBuilder.build();

    this.playerWaitingComponent.SetupHubConnection(this._connection);

    this._connection.onreconnected(() => {
      this.validateStatus().catch(err => {
        this.playerWaitingComponent.HandleGenericError();
        console.error(err);
      });
    });

    this._connection.onclose(() => {
      this.playerWaitingComponent.HubConnectionFailed();
    });

    this.connectHubConnection()
      .then(() => {
        return this.validateStatus()
      })
      .then(response => {
        if (response.success) {
          return this.playerWaitingComponent.LoadPlayers();
        }
      })
      .then(response => {
        if (response && response.success) {
          this.Players = response.data;
        }
        else if (response) {
          this.playerWaitingComponent.ErrorMessage = response.errorCode;
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {
          this.playerWaitingComponent.LoadContent();
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {
          this.playerWaitingComponent.Loading = false;
        }
      })
      .catch(err => {
        this.playerWaitingComponent.HandleGenericError();
        console.error(err);
      });
  }

  constructor(playerStatusService: JustOnePlayerStatusService) {
    this._playerStatusService = playerStatusService;
  }

  private validateStatus(): Promise<GenericResponse<PlayerStatusValidationResponse>> {
    return this._playerStatusService.Validate(this.playerWaitingComponent, this.playerWaitingComponent.GetPlayerStatus(), () => {
      this.playerWaitingComponent.OnRedirect();
      this.CloseHubConnection();
    });
  }

  private connectHubConnection(): Promise<any> {
    return this._connection.start()
      .catch(err => { this.playerWaitingComponent.HubConnectionFailed(); console.log(err); });
  }

  CloseHubConnection() {
    var connection = this._connection;
    if (connection) {

      this.playerWaitingComponent.OnCloseHubConnection(connection);

      connection.onclose(() => { });

      connection.stop().then(() => {
        this._connection = null;
      })
      .catch(err => {
        console.log(err);
      })
    }
  }

  HandlePlayerAction(playerAction: PlayerAction) {
    if (playerAction && playerAction.hasTakenAction && this.Players && this.Players.length > 0) {
      _.find(this.Players, p => {
        return p.id == playerAction.id;
      }).hasTakenAction = true;
    }
  }
  PlayerHasTakenAction(playerId: string, hasTakenAction: boolean) {
    if (playerId) {
      _.find(this.Players, p => {
        return p.id == playerId;
      }).hasTakenAction = hasTakenAction;
    }
  }
}
