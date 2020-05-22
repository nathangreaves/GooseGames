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

@Component({
  selector: 'app-just-one-passiveplayerwaitingforcluevote-component',
  templateUrl: './passiveplayerwaitingforcluevote.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePassivePlayerWaitingForClueVoteComponent extends JustOnePlayerWaitingComponentBase {

  _playerWaitingComponent: JustOnePlayerWaitingComponent;
  _playerActionsService: JustOneRoundService;
  _playerStatusService: JustOnePlayerStatusService;
  _router: Router;

  DisableButtons: boolean = true;

  constructor(router: Router, activatedRoute: ActivatedRoute, playerActionsService: JustOneRoundService, playerStatusService: JustOnePlayerStatusService) {
    super(activatedRoute);
    this._playerActionsService = playerActionsService;
    this._playerStatusService = playerStatusService;
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
    this.DisableButtons = false;
    return Promise.resolve();
  }

  Undo(): void {
    this.DisableButtons = true;
    this._playerStatusService.SetPassivePlayerClueVote(this)
      .then(response => {
        if (response.success) {
          this._playerWaitingComponent.CloseHubConnection();
          this._router.navigate([
            PlayerStatusRoutesMap.PassivePlayerClueVote, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
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

  SetupHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.on("clueVoteSubmitted", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId, true);
    });
    hubConnection.on("clueVoteRevoked", (playerId: string) => {
      this._playerWaitingComponent.PlayerHasTakenAction(playerId, false);
    });
    hubConnection.on("allClueVotesSubmitted", () => {
      this._playerWaitingComponent.CloseHubConnection();
      this._router.navigate([
        PlayerStatusRoutesMap.PassivePlayerWaitingForActivePlayer, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
  }
  OnCloseHubConnection(hubConnection: signalR.HubConnection) {
    hubConnection.off("clueVoteSubmitted");
    hubConnection.off("clueVoteRevoked");
    hubConnection.off("allClueVotesSubmitted");
  }

}  
