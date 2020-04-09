import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../../services/justone/playerdetails'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { JustOneRoundService } from '../../../services/justone/round';
import { RoundInformationResponse } from '../../../models/justone/round';
import { GenericResponse } from '../../../models/genericresponse';
import { NavbarService } from '../../../services/navbar';
import { NavbarHeaderEnum } from '../../nav-menu/navbar-header';

@Component({
  selector: 'app-just-one-roundwaiting-component',
  templateUrl: './waiting.component.html'
})

export class JustOneRoundWaitingComponent implements IPlayerSessionComponent {
  _router: Router;
  _hubConnection: signalR.HubConnection;
  _playerStatusService: JustOnePlayerStatusService
  _roundService: JustOneRoundService;
  _navbarService: NavbarService;

  SessionId: string;
  PlayerId: string;
  Loading: boolean = true;
  ErrorMessage: string;

  constructor(playerStatusService: JustOnePlayerStatusService, roundService: JustOneRoundService, navbarService: NavbarService, router: Router, activatedRoute: ActivatedRoute) {
    this._router = router;
    this._playerStatusService = playerStatusService;
    this._roundService = roundService;
    this._navbarService = navbarService;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.setupConnection();

    this._playerStatusService.Validate(this, PlayerStatus.RoundWaiting, () => { this.CloseConnection(); })
      .then(data => {
        if (data.success) {
          this.Loading = false;
        }
      });
  }

  private setupConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .build();
    this._hubConnection.on("beginRoundPassivePlayer", () => {
      this.CloseConnection();
      this._router.navigate(['/justone/round/submitclue', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.on("beginRoundActivePlayer", () => {
      this.CloseConnection();
      this._router.navigate(['/justone/round/playerwaiting', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.start().catch(err => console.error(err));
  }

  CloseConnection() {
    if (this._hubConnection) {
      this._hubConnection.off("beginRoundPassivePlayer");
      this._hubConnection.off("beginRoundActivePlayer");

      this._hubConnection.stop();
      this._hubConnection = null;
    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
