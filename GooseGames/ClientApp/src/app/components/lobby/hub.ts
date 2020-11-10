import { Component, OnDestroy, OnInit, Input } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { PlayerDetailsResponse } from '../../../models/player';

export interface IGlobalLobbyHubParameters {
  sessionId: string;
  playerId: string;
  handlePlayerAdded: (player: PlayerDetailsResponse) => void;
  handlePlayerDetailsUpdated: (player: PlayerDetailsResponse) => void;
  handlePlayerRemoved: (playerId: string) => void;
  handleConnectionError: (error: string) => void;
  handleReconnected: () => void;
  resolveConnected: () => void;
}

@Component({
  selector: 'global-lobby-hub-component',
  template: ''
})
export class GlobalLobbyHubComponent implements OnInit, OnDestroy {

  @Input() parameters: IGlobalLobbyHubParameters;

  _globalHubConnection: signalR.HubConnection;

  private setupConnection(): Promise<any> {

    this.closeConnection();

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
      this.parameters.resolveConnected();
    });
  }

  ngOnDestroy(): void {
    this.closeConnection();
  }

  closeConnection() {
    var globalConnection = this._globalHubConnection;
    if (globalConnection) {
      this._globalHubConnection = null;
      globalConnection.off("playerAdded");
      globalConnection.off("playerDetailsUpdated");
      globalConnection.off("playerRemoved");

      globalConnection.stop();
    }
  }
}
