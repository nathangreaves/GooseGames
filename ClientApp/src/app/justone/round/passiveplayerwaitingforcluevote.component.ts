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
    _hubConnection: any;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._router = router;
  }

  getPlayerStatus(): PlayerStatus { return PlayerStatus.PassivePlayerWaitingForClueVotes; }

  setPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  loadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerResponseVoteInformation(this);
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
      this._hubConnection.off("clueVoteSubmitted");
      this._hubConnection.off("allClueVotesSubmitted");
    }
  }

  setupConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("clueVoteSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });
    hubConnection.on("allClueVotesSubmitted", () => {
      this.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.PassivePlayerWaitingForActivePlayer, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
}  
