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

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
  }

  GetPlayerStatus(): PlayerStatus { return PlayerStatus.RoundWaiting; }

  SetPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  LoadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayersWaitingForRound(this);
  }

  LoadContent(): Promise<any> {
    return Promise.resolve();
  }

  SetupHubConnection(hubConnection: signalR.HubConnection) {

    hubConnection.on("playerReadyForRound", (playerId :string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });

    hubConnection.on("beginRoundPassivePlayer", () => {
      this._playerWaitingComponent.CloseHubConnection();
      this._router.navigate([PlayerStatusRoutesMap.PassivePlayerClue, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    hubConnection.on("beginRoundActivePlayer", () => {
      this._playerWaitingComponent.CloseHubConnection();
      this._router.navigate([PlayerStatusRoutesMap.ActivePlayerWaitingForClues, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }

  OnCloseHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.off("playerReadyForRound");
    hubConnection.off("beginRoundPassivePlayer");
    hubConnection.off("beginRoundActivePlayer");
  }

}  
