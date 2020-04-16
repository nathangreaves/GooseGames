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
  selector: 'app-just-one-passiveplayerwaitingforcluevote-component',
  templateUrl: './passiveplayerwaitingforcluevote.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePassivePlayerWaitingForClueVoteComponent extends JustOnePlayerWaitingComponentBase {

  _playerWaitingComponent: JustOnePlayerWaitingComponent;
  _playerActionsService: JustOneRoundService;
  _router: Router;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
  }

  GetPlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerWaitingForClueVotes; }

  SetPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  LoadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerResponseVoteInformation(this);
  }

  LoadContent(): Promise<any> {
    return Promise.resolve();
  }

  SetupHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("clueVoteSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });
    hubConnection.on("allClueVotesSubmitted", () => {
      this._playerWaitingComponent.CloseHubConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.PassivePlayerWaitingForActivePlayer, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
  OnCloseHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.off("clueVoteSubmitted");
    hubConnection.off("allClueVotesSubmitted");
  }

}  
