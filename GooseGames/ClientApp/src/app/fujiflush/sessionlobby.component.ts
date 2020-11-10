import { Component, OnInit, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { FujiPlayerDetailsService } from '../../services/fujiflush/playerdetails'
import { FujiSessionService } from '../../services/fujiflush/session'
import { GenericResponseBase } from '../../models/genericresponse'
import { PlayerDetailsResponse } from '../../models/player'
import { Router, ActivatedRoute } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { IPlayerSessionComponent } from '../../models/session';
import { ILobbyComponentParameters } from '../components/lobby/lobby';

const MinPlayers: number = 3;
const MaxPlayers: number = 8;

@Component({
  selector: 'app-fuji-sessionlobby-component',
  templateUrl: './sessionlobby.component.html',
  styleUrls: ['./sessionlobby.component.css'],
})
export class FujiSessionLobbyComponent implements IPlayerSessionComponent, OnInit, OnDestroy {

  private _sessionService: FujiSessionService;
  private _router: Router;
  private _hubConnection: signalR.HubConnection;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;


  public Loading: boolean;
  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;
  public Players: PlayerDetailsResponse[];

  lobbyParameters: ILobbyComponentParameters;

  constructor(sessionService: FujiSessionService, router: Router, activatedRoute: ActivatedRoute) {
    this._sessionService = sessionService;
    this._router = router;

    this.Loading = true;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.setupConnection();
  }

  ngOnInit(): void {

    this.lobbyParameters = {
      canStartSession: () => true,
      minPlayers: MinPlayers,
      maxPlayers: MaxPlayers,
      playerId: this.PlayerId,
      sessionId: this.SessionId,
      startSession: this.startSession
    }
  }
  ngOnDestroy(): void {
    this.CloseConnection();
  }

  startSession = (): Promise<GenericResponseBase> => {
    return this._sessionService.StartSession(this);
  }

  private setupConnection(): Promise<any> {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/fujihub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {

    });
    this._hubConnection.onclose(() => {
      this.HandleGenericError();
    });

    this._hubConnection.on("startingSession", () => {
      this.CloseConnection();
      this._router.navigate(['/fujiflush/waiting', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.on("beginSession", () => {
      this.CloseConnection();
      this._router.navigate(['/fujiflush/session', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    return this._hubConnection.start().catch(err => console.error(err));
  }

  CloseConnection() {
    var connection = this._hubConnection;
    if (connection) {
      this._hubConnection = null;
      connection.off("startingSession");
      connection.off("beginSession");

      connection.onclose(() => { });

      connection.stop();
    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
