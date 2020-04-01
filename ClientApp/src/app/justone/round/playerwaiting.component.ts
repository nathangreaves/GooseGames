import * as _ from 'lodash';
import { Component, Input, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { PlayerAction } from '../../../models/justone/playeractions';
import { GenericResponse } from '../../../models/genericresponse';
import { PlayerNumberCss } from '../../../services/justone/ui'

export interface IJustOnePlayerWaitingComponent extends IPlayerSessionComponent {
  getPlayerStatus(): PlayerStatus;
  loadPlayers(): Promise<GenericResponse<PlayerAction[]>>;
  loadContent(): Promise<any>;
  onRedirect();
  createConnection();
  setPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent);

  Loading: boolean;
}

export abstract class JustOnePlayerWaitingComponentBase implements IJustOnePlayerWaitingComponent {
  abstract getPlayerStatus(): PlayerStatus;
  abstract loadPlayers(): Promise<GenericResponse<PlayerAction[]>>;
  abstract loadContent(): Promise<any>;
  abstract onRedirect();
  abstract setPlayerWaitingComponent(playerWaitingComponent: JustOnePlayerWaitingComponent);
  abstract createConnection();

  constructor(activatedRoute: ActivatedRoute) {
    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;
  }

  HandleGenericError() {
    this.ErrorMessage = "An unknown error occurred";
  }

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;
}

@Component({
  selector: 'app-just-one-playerwaiting-component',
  templateUrl: './playerwaiting.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePlayerWaitingComponent implements OnInit  {

  PlayerNumberCss = PlayerNumberCss;

  private _playerStatusService: JustOnePlayerStatusService;

  Players: PlayerAction[];
  @Input() playerWaitingComponent: IJustOnePlayerWaitingComponent;

  ngOnInit() {
    this.playerWaitingComponent.setPlayerWaitingComponent(this);

    this._playerStatusService.Validate(this.playerWaitingComponent,
      this.playerWaitingComponent.getPlayerStatus(), () => {
        this.playerWaitingComponent.onRedirect();
      })
      .then(response => {
        if (response.success) {
          return this.playerWaitingComponent.loadPlayers();
        }
      })
      .then(response => {
        if (response && response.success) {
          this.Players = response.data;
        }
        else if (response){
          this.playerWaitingComponent.ErrorMessage = response.errorCode;
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {
          this.playerWaitingComponent.loadContent();
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {

          this.playerWaitingComponent.createConnection();
          this.playerWaitingComponent.Loading = false;
        }
      })
      .catch(() => {
        this.playerWaitingComponent.HandleGenericError();
      });
  }

  constructor(playerStatusService: JustOnePlayerStatusService) {
    this._playerStatusService = playerStatusService;  
  }
  HandlePlayerAction(playerAction: PlayerAction) {
    if (playerAction && playerAction.hasTakenAction) {
      _.find(this.Players, p => {
        return p.id == playerAction.id;
      }).hasTakenAction = true;
    }
  }
  PlayerHasTakenAction(playerId: string) {
    if (playerId) {
      _.find(this.Players, p => {
        return p.id == playerId;
      }).hasTakenAction = true;
    }
  }  
}
