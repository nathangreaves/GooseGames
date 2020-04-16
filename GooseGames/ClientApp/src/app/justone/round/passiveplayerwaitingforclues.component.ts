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

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
  }

  GetPlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerWaitingForClues; }

  SetPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  LoadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerResponseInformation(this);
  }

  LoadContent(): Promise<any> {
    return Promise.resolve();
  }
  SetupHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("clueSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });
    hubConnection.on("allCluesSubmitted", () => {
      this._playerWaitingComponent.CloseHubConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.PassivePlayerClueVote, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }

  OnCloseHubConnection(connection: signalR.HubConnection) {
    connection.off("clueSubmitted");
    connection.off("allCluesSubmitted");
  }

}  
