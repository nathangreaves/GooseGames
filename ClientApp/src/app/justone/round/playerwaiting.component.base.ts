import * as _ from 'lodash';
import { Router, ActivatedRoute } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { PlayerAction } from '../../../models/justone/playeractions';
import { GenericResponse } from '../../../models/genericresponse';

export abstract class JustOnePlayerWaitingComponentBase implements IPlayerSessionComponent {

  private _router: Router;
  private _hubConnection: signalR.HubConnection;
  private _playerStatusService: JustOnePlayerStatusService;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;

  Players: PlayerAction[];

  constructor(playerStatusService: JustOnePlayerStatusService, router: Router, activatedRoute: ActivatedRoute, playerStatus: PlayerStatus) {
    this._router = router;
    this._playerStatusService = playerStatusService;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();
    this.setupConnection(this._hubConnection);
    this._hubConnection.start().catch(err => console.error(err));

    this._playerStatusService.Validate(this, playerStatus, () => {
      this.onRedirect();
      this.CloseConnection();
    })
      .then(response => {
        if (response.success) {
          return this.loadPlayers();
        }
      })
      .then(response => {
        if (response.success) {
          this.Players = response.data;
        }
        else {
          this.ErrorMessage = response.errorCode;
        }
      })
      .then(() => {
        this.Loading = false;
      })
      .catch(() =>
      {
        this.HandleGenericError();
      });
  }

  abstract loadPlayers(): Promise<GenericResponse<PlayerAction[]>>;
  abstract onRedirect();
  abstract setupConnection(connection: signalR.HubConnection);

  HandlePlayerAction(playerAction: PlayerAction) {
    if (playerAction && playerAction.hasTakenAction) {
      _.find(this.Players, p =>
      {
        p.id == playerAction.id;
        p.hasTakenAction = true;
      })
    }
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
