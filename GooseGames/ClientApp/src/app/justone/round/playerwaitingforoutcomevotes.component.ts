import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { PlayerAction } from '../../../models/justone/playeractions'
import { JustOnePlayerWaitingComponentBase, JustOnePlayerWaitingComponent } from './playerwaiting.component';
import { JustOneRoundService } from '../../../services/justone/round';
import { GenericResponse } from '../../../models/genericresponse';

export abstract class JustOnePlayerWaitingForOutcomeVoteComponentBase extends JustOnePlayerWaitingComponentBase {

  _playerWaitingComponent: JustOnePlayerWaitingComponent;
  _playerActionsService: JustOneRoundService;
  _router: Router;

  IsActivePlayer: boolean;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
    this.IsActivePlayer = this.isActivePlayer();
  }

  abstract getRoundOutcomePlayerStatus(): PlayerStatus;
  abstract isActivePlayer(): boolean;

  SetPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  LoadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerOutcomeVoteInformation(this);
  }

  LoadContent(): Promise<any> {
    return Promise.resolve();
  }

  SetupHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("responseVoteSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });
    hubConnection.on("roundOutcomeAvailable", () => {
      this._playerWaitingComponent.CloseHubConnection();

      var statusName = PlayerStatus[this.getRoundOutcomePlayerStatus()];

      this._router.navigate([
        PlayerStatusRoutesMap[statusName], { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
  OnCloseHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.off("responseVoteSubmitted");
    hubConnection.off("roundOutcomeAvailable");
  }
}  

@Component({
  selector: 'app-just-one-passiveplayerwaitingforoutcomevotes-component',
  templateUrl: './playerwaitingforoutcomevotes.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePassivePlayerWaitingForOutcomeVoteComponent extends JustOnePlayerWaitingForOutcomeVoteComponentBase {
  GetPlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerWaitingForOutcomeVotes; }
  getRoundOutcomePlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerOutcome; }
  isActivePlayer(): boolean { return false; }

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(router, activatedRoute, playerActionsService);
  }
}

@Component({
  selector: 'app-just-one-activeplayerwaitingforoutcomevotes-component',
  templateUrl: './playerwaitingforoutcomevotes.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOneActivePlayerWaitingForOutcomeVoteComponent extends JustOnePlayerWaitingForOutcomeVoteComponentBase {
  GetPlayerStatus(): PlayerStatus { return PlayerStatus.ActivePlayerWaitingForOutcomeVotes; }
  getRoundOutcomePlayerStatus(): PlayerStatus { return PlayerStatus.ActivePlayerOutcome; }
  isActivePlayer(): boolean { return true; }

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(router, activatedRoute, playerActionsService);
  }
}
