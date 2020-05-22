import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { PlayerAction } from '../../../models/player'
import { JustOnePlayerWaitingComponentBase, JustOnePlayerWaitingComponent } from './playerwaiting.component';
import { JustOneRoundService } from '../../../services/justone/round';
import { GenericResponse } from '../../../models/genericresponse';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus';

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
      this._playerWaitingComponent.PlayerHasTakenAction(playerId, true);
    });
    hubConnection.on("responseVoteRevoked", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId, false);
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
    hubConnection.off("responseVoteRevoked");
    hubConnection.off("roundOutcomeAvailable");
  }
}  

@Component({
  selector: 'app-just-one-passiveplayerwaitingforoutcomevotes-component',
  templateUrl: './playerwaitingforoutcomevotes.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePassivePlayerWaitingForOutcomeVoteComponent extends JustOnePlayerWaitingForOutcomeVoteComponentBase {

  _playerStatusService: JustOnePlayerStatusService;

  DisableButtons: boolean = true;

  GetPlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerWaitingForOutcomeVotes; }
  getRoundOutcomePlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerOutcome; }
  isActivePlayer(): boolean { return false; }

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService, playerStatusService: JustOnePlayerStatusService) {
    super(router, activatedRoute, playerActionsService);
    this._playerStatusService = playerStatusService;
  }

  LoadContent(): Promise<any> {
    this.DisableButtons = false;
    return Promise.resolve();
  }

  Undo(): void {
    this.DisableButtons = true;
    this._playerStatusService.SetPassivePlayerOutcomeVote(this)
      .then(response => {
        if (response.success) {
          this._playerWaitingComponent.CloseHubConnection();
          this._router.navigate([
            PlayerStatusRoutesMap.PassivePlayerClue, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
        else {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(err => {
        console.error(err);
        this.HandleGenericError();
      });
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
