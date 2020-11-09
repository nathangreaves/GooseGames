import { Component, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";

import { IPlayerSessionComponent } from '../../models/session';

import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { NavbarService } from '../../services/navbar';
import { NavbarHeaderEnum } from '../nav-menu/navbar-header';

@Component({
  selector: 'app-fuji-roundwaiting-component',
  templateUrl: './waiting.component.html'
})

export class FujiWaitingComponent implements IPlayerSessionComponent, OnDestroy {
  _router: Router;
  _hubConnection: signalR.HubConnection; 

  SessionId: string;
  PlayerId: string;
  Loading: boolean = true;
  ErrorMessage: string;

  constructor( router: Router, activatedRoute: ActivatedRoute) {
    this._router = router;


    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.setupConnection().then(() => {

      //TODO: Some form of validation?
      //return this._playerStatusService.Validate(this, PlayerStatus.RoundWaiting, () => { this.CloseConnection(); })
      //  .then(data => {
      //    if (data.success) {
      //    }
      //  })
      this.Loading = false;      
    })
    .catch((err) => {
      console.error(err);
      this.HandleGenericError();
    });
  }

  ngOnDestroy(): void {
    this.CloseConnection();
  }

  private setupConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/fujihub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {
      //TODO: Validate?
      //this.Validate();
    });
    this._hubConnection.onclose(() => {
      this.HandleGenericError();
    });

    this._hubConnection.on("beginSession", () => {
      this.CloseConnection();
      this._router.navigate(['/fujiflush/session', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    return this._hubConnection.start().catch(err => console.error(err));
  }
  //Validate(): Promise<GenericResponseBase> {
  //  return this._playerStatusService.Validate(this, PlayerStatus.RoundWaiting, () => { this.CloseConnection(); });
  //}

  CloseConnection() {
    var connection = this._hubConnection;

    if (connection) {
    this._hubConnection = null;
      connection.off("beginSession");

      connection.onclose(() => { });
      connection.stop();

    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
