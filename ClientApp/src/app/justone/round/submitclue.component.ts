import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../../services/justone/playerdetails'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';

@Component({
  selector: 'app-just-one-submitclue-component',
  templateUrl: './submitclue.component.html'
})

export class JustOneSubmitClueComponent {
  private _router: Router;

  private SessionId: string;
  private PlayerId: string;

  private _hubConnection: signalR.HubConnection;

  public ErrorMessage: string;
  public Loading: boolean;

  constructor(router: Router, activatedRoute: ActivatedRoute) {
    this._router = router;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.setupConnection();

    this.validatePlayerStatus();
  }

  private validatePlayerStatus() {
    //TODO: If not valid redirect and close connection
  }

  private setupConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();
    //this._hubConnection.on("beginRoundPassivePlayer", () => {
    //  this._hubConnection.stop();
    //  this._hubConnection = null;
    //  this._router.navigate(['/justone/submitclue', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    //});
    this._hubConnection.start().catch(err => console.error(err));
  }

  handleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
