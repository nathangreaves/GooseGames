import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { PlayerAction } from '../../../models/justone/playeractions'
import { JustOnePlayerWaitingComponentBase, JustOnePlayerWaitingComponent } from './playerwaiting.component';
import { JustOneRoundService } from '../../../services/justone/round';
import { GenericResponse } from '../../../models/genericresponse';

@Component({
  selector: 'app-just-one-waitingforround-component',
  templateUrl: './waitingforround.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOneWaitingForRoundComponent extends JustOnePlayerWaitingComponentBase {

  _playerWaitingComponent: JustOnePlayerWaitingComponent;
  _playerActionsService: JustOneRoundService;
  _router: Router;
  _hubConnection: any;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
  }

  getPlayerStatus(): PlayerStatus { return PlayerStatus.RoundWaiting; }

  setPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  loadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayersWaitingForRound(this);
  }

  loadContent(): Promise<any> {
    return Promise.resolve();
  }

  createConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();
    this.setupConnection(this._hubConnection);
    this._hubConnection.start().catch(err => console.error(err));
  }
  onRedirect() {
    this.CloseConnection();
  }
  CloseConnection() {
    if (this._hubConnection) {
      this._hubConnection.off("beginRoundPassivePlayer");
      this._hubConnection.off("beginRoundActivePlayer");

      this._hubConnection.stop();
      this._hubConnection = null;
    }
  }

  setupConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("beginRoundPassivePlayer", () => {
      this.CloseConnection();
      this._router.navigate([PlayerStatusRoutesMap.PassivePlayerClue, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    hubConnection.on("beginRoundActivePlayer", () => {
      this.CloseConnection();
      this._router.navigate([PlayerStatusRoutesMap.ActivePlayerWaitingForClues, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
}  
