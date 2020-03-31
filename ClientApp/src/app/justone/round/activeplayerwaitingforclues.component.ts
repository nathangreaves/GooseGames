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
  selector: 'app-just-one-activeplayerwaitingforclues-component',
  templateUrl: './activeplayerwaitingforclues.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOneActivePlayerWaitingForCluesComponent extends JustOnePlayerWaitingComponentBase { 

  _playerWaitingComponent: JustOnePlayerWaitingComponent; 
  _playerActionsService: JustOneRoundService;
    _router: Router;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService)
  {
    super(activatedRoute);
    this._router = router;
    this._playerActionsService = playerActionsService;
  }

  getPlayerStatus(): PlayerStatus { return PlayerStatus.ActivePlayerWaitingForClues; }

  setPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent) {
    this._playerWaitingComponent = playerWaitingComponent;
  }

  loadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerResponseInformation(this);
  }

  loadContent(): Promise<any> {
    return Promise.resolve();
  }

  onRedirect() {

  }

  setupConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("clueSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId);
    });
    hubConnection.on("allCluesSubmitted", () => {
      this._playerWaitingComponent.CloseConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.ActivePlayerWaitingForVotes, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
}  
