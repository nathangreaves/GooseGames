import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { PlayerAction } from '../../../models/justone/playeractions'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { JustOnePlayerWaitingComponentBase } from './playerwaiting.component.base';
import { JustOnePlayerActionsService } from '../../../services/justone/playeractions';
import { GenericResponse } from '../../../models/genericresponse';

@Component({
  selector: 'app-just-one-activeplayerwaitingforclues-component',
  templateUrl: './activeplayerwaitingforclues.component.html',
  styleUrls: ['../sessionlobby.component.css']
})

export class JustOneActivePlayerWaitingForCluesComponent extends JustOnePlayerWaitingComponentBase implements IPlayerSessionComponent {

  _playerActionsService: JustOnePlayerActionsService;

  constructor(playerStatusService: JustOnePlayerStatusService, router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOnePlayerActionsService)
  {
    super(playerStatusService, router, activatedRoute, PlayerStatus.ActivePlayerWaitingForClues);

    this._playerActionsService = playerActionsService;
  }

  loadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this._playerActionsService.GetPlayerResponseInformation(this);
  }

  onRedirect() {

  }

  setupConnection(hubConnection: signalR.HubConnection) {

    hubConnection.on("playerVoted", (playerAction: PlayerAction) => {
      this.HandlePlayerAction(playerAction);
    });

    //this._hubConnection.on("beginRoundActivePlayer", () => {
    //  this._hubConnection.stop();
    //  this._hubConnection = null;
    //  this._router.navigate(['/justone/playerwaiting', { sessionId: this._sessionId, playerId: this._playerId }]);
    //});
  }
}
