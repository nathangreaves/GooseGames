import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';

@Component({
  selector: 'app-just-one-passiveplayerwaitingforactiveplayer-component',
  templateUrl: './passiveplayerwaitingforactiveplayer.component.html'
})

export class JustOnePassivePlayerWaitingForActivePlayerComponent implements IPlayerSessionComponent {

  _playerStatusService: JustOnePlayerStatusService;
  _router: Router;

  _hubConnection: signalR.HubConnection;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;

  public Loading: boolean;

  constructor(playerStatusService: JustOnePlayerStatusService, router: Router, activatedRoute: ActivatedRoute) {
    this._playerStatusService = playerStatusService;
    this._router = router;

    this.Loading = true;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.setupConnection();

    this._playerStatusService.Validate(this, PlayerStatus.PassivePlayerWaitingForActivePlayer, () => { })
      .then(response => { if (response.success) { return this.load(); } })
      .catch(() => this.HandleGenericError());
  }

  private load() {
    this.Loading = false;
    return Promise.resolve();
  }

  private setupConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();
    this._hubConnection.on("activePlayerGuessed", () => {
      this.CloseConnection();

    });
    this._hubConnection.start().catch(err => console.error(err));
  }
  CloseConnection() {
    if (this._hubConnection) {
      this._hubConnection.stop();
      this._hubConnection = null;
    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
