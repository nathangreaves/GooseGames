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
  selector: 'app-just-one-passiveplayerwaitingforclues-component',
  templateUrl: './passiveplayerwaitingforclues.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePassivePlayerWaitingForCluesComponent extends JustOnePlayerWaitingComponentBase {

  _playerWaitingComponent: JustOnePlayerWaitingComponent;
  _playerActionsService: JustOneRoundService;
  _router: Router;
    _hubConnection: any;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
  }

  getPlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerWaitingForClues; }

  setPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  loadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerResponseInformation(this);
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
      this._hubConnection.off("clueSubmitted");
      this._hubConnection.off("allCluesSubmitted");
      this._hubConnection.stop();
      this._hubConnection = null;
    }
  }

  setupConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("clueSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });
    hubConnection.on("allCluesSubmitted", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.PassivePlayerClueVote, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
}  
