import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { JustOneClueListComponentBase, JustOneClueListComponent } from './cluelist.component';
import { GenericResponse, GenericResponseBase } from '../../../models/genericresponse';
import { PlayerCluesResponse } from '../../../models/justone/clue';
import { JustOneClueService } from '../../../services/justone/clue';
import { PlayerNumberCss } from '../../../services/justone/ui'

@Component({
  selector: 'app-just-one-passiveplayerwaitingforactiveplayer-component',
  templateUrl: './passiveplayerwaitingforactiveplayer.component.html'
})

export class JustOnePassivePlayerWaitingForActivePlayerComponent extends JustOneClueListComponentBase {
  PlayerNumberCss = PlayerNumberCss;
  _clueService: JustOneClueService;
  _clueListComponent: JustOneClueListComponent;
  _hubConnection: signalR.HubConnection;
  _router: Router;
  ActivePlayerNumber: number;
  ActivePlayerName: string;
  Word: string;
  RevealedWord: string;

  constructor(clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {

    super(activatedRoute);
    this._router = router;
    this._clueService = clueService;

    this.hide();
  }
  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.PassivePlayerWaitingForActivePlayer;
  }
  isClueListReadOnly(): boolean {
    return true;
  }

  preValidate(): Promise<any> {
    return this.setupConnection();
  }

  show() {
    this.Word = this.RevealedWord;
  }

  hide() {
    this.Word = "PRESS TO REVEAL";
  }

  loadClues(): Promise<GenericResponse<PlayerCluesResponse>> {
    return this._clueService.GetClues(this).then(response => {
      if (response.success) {
        this.ActivePlayerNumber = response.data.activePlayerNumber;
        this.ActivePlayerName = response.data.activePlayerName;
        this.RevealedWord = response.data.wordToGuess;
      }
      return response;
    });
  }
  loadContent(): Promise<GenericResponseBase> {
    return Promise.resolve(
      {
        success: true,
        errorCode: null,
      });
  }
  MarkClueAs() {
    //Do nothing as this is a readonly clue list
  }
  onRedirect() {
    this.CloseConnection();
  }
  setClueListComponent(clueListComponent: JustOneClueListComponent) {
    this._clueListComponent = clueListComponent;
  }

  private setupConnection() : Promise<any> {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {
      this._clueListComponent.Validate()
        .catch(err => {
          console.error(err);
          this.HandleGenericError();
        })
    });
    this._hubConnection.onclose(() =>
    {
      this.HandleGenericError();
    });

    this._hubConnection.on("roundOutcomeAvailable", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.PassivePlayerOutcome, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.on("activePlayerResponseVoteRequired", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.PassivePlayerOutcomeVote, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    return this._hubConnection.start().catch(err => { this.HandleGenericError(); console.error(err) });
  }
  CloseConnection() {
    if (this._hubConnection) {
      this._hubConnection.off("roundOutcomeAvailable");
      this._hubConnection.off("activePlayerResponseVoteRequired");

      this._hubConnection.onclose(() => { });
      this._hubConnection.stop().then(() => {
        this._hubConnection = null;
      })
        .catch(err => {
          console.error(err);
          this.HandleGenericError();
        });
    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
