import { Component } from '@angular/core';
import * as _ from 'lodash';
import * as signalR from "@microsoft/signalr";
import { Router, ActivatedRoute } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { JustOneClueService } from '../../../services/justone/clue';
import { JustOneClueListComponentBase, JustOneClueListComponent } from './cluelist.component';
import { GenericResponse, GenericResponseBase } from '../../../models/genericresponse';
import { PlayerCluesResponse } from '../../../models/justone/clue';

@Component({
  selector: 'app-just-one-activeplayerguess-component',
  templateUrl: './activeplayerguess.component.html',
  styleUrls: ['./activeplayerguess.component.css']
})

export class JustOneActivePlayerGuess extends JustOneClueListComponentBase {

  _router: Router;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;

  public Loading: boolean;
  _clueService: JustOneClueService;
  _clueListComponent: JustOneClueListComponent;
    _hubConnection: any;

  constructor(clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {

    super(activatedRoute);
    this._router = router;
    this._clueService = clueService;
  }

  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.ActivePlayerGuess;
  }
  isClueListReadOnly(): boolean {
    return true;
  }
  loadClues(): Promise<GenericResponse<PlayerCluesResponse>> {
    return this._clueService.GetClues(this);
  }
  loadContent(): Promise<GenericResponseBase> {

    this.setupConnection();

    return Promise.resolve(
      {
        success: true,
        errorCode: null,
      });
  }
  MarkClueAsInvalid() {
  }
  onRedirect() {
  }
  setClueListComponent(clueListComponent: JustOneClueListComponent) {
    this._clueListComponent = clueListComponent;
  }

  SubmitResponse(response: string) {
    if (!response) {
      this.ErrorMessage = "Please submit response";
      return;
    }
    if (response.indexOf(" ") != -1) {
      this.ErrorMessage = "Please submit JUST ONE word";
      return;
    }

    this._clueService.SubmitActivePlayerResponse({
      SessionId: this.SessionId,
      PlayerId: this.PlayerId,
      ResponseWord: response,
      Pass: false
    })
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(() => this.HandleGenericError());
  }

  SubmitPass(response: string) {
    this._clueService.SubmitActivePlayerResponse({
      SessionId: this.SessionId,
      PlayerId: this.PlayerId,
      ResponseWord: response,
      Pass: true
    })
      .then(response => {
      if (!response.success) {
        this.ErrorMessage = response.errorCode;
      }
    })
      .catch(() => this.HandleGenericError());
  }

  private setupConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();
    this._hubConnection.on("roundOutcomeAvailable", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap[PlayerStatus.ActivePlayerOutcome], { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.on("activePlayerResponseVoteRequired", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap[PlayerStatus.ActivePlayerWaitingForOutcomeVotes], { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.start().catch(err => console.error(err));
  }
  CloseConnection() {
    if (this._hubConnection) {
      this._hubConnection.stop();
      this._hubConnection = null;
    }
  }
}
