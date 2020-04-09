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

  IsActivePlayer: any;
    _hubConnection: signalR.HubConnection;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
    this.IsActivePlayer = this.isActivePlayer();
  }

  setPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  loadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerOutcomeVoteInformation(this);
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
      this._hubConnection.off("responseVoteSubmitted");
      this._hubConnection.off("roundOutcomeAvailable");

      this._hubConnection.stop();
      this._hubConnection = null;
    }
  }

  abstract getRoundOutcomePlayerStatus(): PlayerStatus;
  abstract isActivePlayer(): boolean;

  setupConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("responseVoteSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });
    hubConnection.on("roundOutcomeAvailable", () => {
      this.CloseConnection();

      var statusName = PlayerStatus[this.getRoundOutcomePlayerStatus()];

      this._router.navigate([
        PlayerStatusRoutesMap[statusName], { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
}  

@Component({
  selector: 'app-just-one-passiveplayerwaitingforoutcomevotes-component',
  templateUrl: './playerwaitingforoutcomevotes.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePassivePlayerWaitingForOutcomeVoteComponent extends JustOnePlayerWaitingForOutcomeVoteComponentBase {
  getPlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerWaitingForOutcomeVotes; }
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
  getPlayerStatus(): PlayerStatus { return PlayerStatus.ActivePlayerWaitingForOutcomeVotes; }
  getRoundOutcomePlayerStatus(): PlayerStatus { return PlayerStatus.ActivePlayerOutcome; }
  isActivePlayer(): boolean { return true; }

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(router, activatedRoute, playerActionsService);
  }
}
