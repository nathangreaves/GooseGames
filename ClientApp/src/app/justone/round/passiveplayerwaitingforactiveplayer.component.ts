import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { JustOneClueListComponentBase, JustOneClueListComponent } from './cluelist.component';
import { GenericResponse, GenericResponseBase } from '../../../models/genericresponse';
import { PlayerCluesResponse } from '../../../models/justone/clue';
import { JustOneClueService } from '../../../services/justone/clue';

@Component({
  selector: 'app-just-one-passiveplayerwaitingforactiveplayer-component',
  templateUrl: './passiveplayerwaitingforactiveplayer.component.html'
})

export class JustOnePassivePlayerWaitingForActivePlayerComponent extends JustOneClueListComponentBase {

  _clueService: JustOneClueService;
  _clueListComponent: JustOneClueListComponent;
  _hubConnection: signalR.HubConnection;
  _router: Router;
    ActivePlayerNumber: number;
    ActivePlayerName: string;
    Word: string;

  constructor(clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {

    super(activatedRoute);
    this._router = router;
    this._clueService = clueService;

    this.setupConnection();
  }
  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.PassivePlayerWaitingForActivePlayer;
  }
  isClueListReadOnly(): boolean {
    return true;
  }
  loadClues(): Promise<GenericResponse<PlayerCluesResponse>> {
    return this._clueService.GetClues(this).then(response => {
      if (response.success) {
        this.ActivePlayerNumber = response.data.activePlayerNumber;
        this.ActivePlayerName = response.data.activePlayerName;
        this.Word = response.data.wordToGuess;
      }
      return response;
    });
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
    //Do nothing as this is a readonly clue list
  }
  onRedirect() {
    this.CloseConnection();
  }
  setClueListComponent(clueListComponent: JustOneClueListComponent) {
    this._clueListComponent = clueListComponent;
  }

  private setupConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();
    this._hubConnection.on("roundOutcomeAvailable", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap[PlayerStatus.PassivePlayerOutcome], { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.on("activePlayerResponseVoteRequired", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap[PlayerStatus.PassivePlayerOutcomeVote], { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
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
