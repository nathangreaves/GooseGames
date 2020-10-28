import { Component, OnDestroy, OnInit, Input } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { PlayerDetailsResponse } from '../../../models/player';

export interface IGlobalLobbyHubParameters {
  sessionId: string;
  playerId: string;
  handlePlayerAdded: (player: PlayerDetailsResponse) => void;
  handlePlayerDetailsUpdated: (player: PlayerDetailsResponse) => void;
  handlePlayerRemoved: (playerId: PlayerDetailsResponse) => void;
  handleConnectionError: (error: string) => void;
  handleReconnected: () => void;
}

@Component({
  selector: 'global-lobby-hub-component',
  template: ''
})
export class GlobalLobbyHubComponent implements OnInit, OnDestroy {

  @Input() parameters: IGlobalLobbyHubParameters;

  _globalHubConnection: signalR.HubConnection;

  private setupConnection(): Promise<any> {

    if (this._globalHubConnection) {
      var oldConnection = this._globalHubConnection;
      oldConnection.onclose(() => { });
      oldConnection.stop();
    }

    this._globalHubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/globalhub?sessionId=${this.parameters.sessionId}&playerId=${this.parameters.playerId}`)
      .withAutomaticReconnect()
      .build();

    this._globalHubConnection.onreconnected(() => {
      this.parameters.handleReconnected();
    });
    this._globalHubConnection.onclose(() => {
      this.parameters.handleConnectionError("connection closed");
    });

    return this._globalHubConnection.start().catch(err => console.error(err));
  }

  ngOnInit(): void {
    this.setupConnection().then(() => {
      this._globalHubConnection.on("playerAdded", this.parameters.handlePlayerAdded);
      this._globalHubConnection.on("playerDetailsUpdated", this.parameters.handlePlayerDetailsUpdated);
      this._globalHubConnection.on("playerRemoved", this.parameters.handlePlayerRemoved);
    });
  }

  ngOnDestroy(): void {
    this.closeConnection();
  }

  closeConnection() {
    var globalConnection = this._globalHubConnection;
    if (globalConnection) {
      globalConnection.off("playerAdded");
      globalConnection.off("playerDetailsUpdated");
      globalConnection.off("playerRemoved");
    }
  }
}
