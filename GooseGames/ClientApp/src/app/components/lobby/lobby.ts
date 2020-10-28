import { Component, OnDestroy, OnInit, Input, TemplateRef } from '@angular/core';
import * as _ from 'lodash';
import { PlayerDetailsResponse } from '../../../models/player';
import { GlobalSessionService } from '../../../services/session';
import { GenericResponseBase } from '../../../models/genericresponse';
import { IGlobalLobbyHubParameters } from './hub';

export interface ILobbyComponentParameters {  
  minPlayers: number;
  maxPlayers: number;
  sessionId: string;
  playerId: string;
  canStartSession: () => boolean;
  startSession: (sessionId: string, playerId: string) => Promise<GenericResponseBase>;
}

@Component({
  selector: 'global-lobby-component',
  templateUrl: './lobby.html',
  styleUrls: ['./lobby.css'],
})
export class LobbyComponent {

  @Input() parameters: ILobbyComponentParameters;
  @Input() gameConfigTemplate: TemplateRef<any>;
  @Input() readOnlyGameConfigTemplate: TemplateRef<any>;

  public Loading: boolean;
  public ErrorMessage: string;
  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;
  public Players: PlayerDetailsResponse[];

  DisableButtons: boolean;
  StatusText: string;

  globalHubConnectionParameters: IGlobalLobbyHubParameters;

  constructor(private _globalSessionService: GlobalSessionService) {

    this.Loading = true;

  }

  playerAdded = (player: PlayerDetailsResponse) => {
    this.setDefaultNewPlayerName(player);
    this.Players.push(player);
  }

  playerDetailsUpdated = (player: PlayerDetailsResponse) => {
    if (this.Players && this.Players.length > 0) {
      var index = _.findIndex(this.Players, p => p.id == player.id);
      this.Players.splice(index, 1, player);
    }
    if (player.isSessionMaster && !this.SessionMaster) {
      this.SessionMasterName = player.playerName;
    }
  }

  playerRemoved = (player: PlayerDetailsResponse) => {
    if (this.Players && this.Players.length > 0) {
      var index = _.findIndex(this.Players, p => p.id == player.id);
      this.Players.splice(index, 1, player);
    }
  }

  handleGenericError(err: string) {
    this.ErrorMessage = err;
  }

  ngOnInit(): void {

    this.globalHubConnectionParameters = {
      playerId: this.parameters.playerId,
      sessionId: this.parameters.sessionId,
      handleConnectionError: this.handleGenericError,
      handleReconnected: () => { },
      handlePlayerAdded: this.playerAdded,
      handlePlayerDetailsUpdated: this.playerDetailsUpdated,
      handlePlayerRemoved: this.playerRemoved
    };

    this.load();
  }

  public StartGame() {

    this.ErrorMessage = null;
    if (_.find(this.Players, p => p.playerNumber == 0 || !p.ready)) {
      this.ErrorMessage = "Not all players are ready";
      return;
    }

    this.DisableButtons = true;    
    this.parameters.startSession(this.parameters.sessionId, this.parameters.playerId)
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch((err) => {
        this.handleGenericError(err);
        this.DisableButtons = false;
      });
  }

  public KickPlayer(playerId: string) {
    if (playerId == this.parameters.playerId) {
      return;
    }
    if (!this.SessionMaster) {
      return;
    }
    this.DisableButtons = true;
    this._globalSessionService.deletePlayer({ sessionMasterId: this.parameters.playerId, playerToDeleteId: playerId })
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch((err) => this.handleGenericError(err))
      .finally(() => this.DisableButtons = false);
  }

  private load() {
    this._globalSessionService.getPlayerDetails({ PlayerId: this.parameters.playerId, SessionId: this.parameters.sessionId })
      .then(data => {
        if (data.success) {

          this.SessionMaster = data.data.sessionMaster;
          this.SessionMasterName = data.data.sessionMasterName ? data.data.sessionMasterName : "Session Master";
          this.SessionMasterPlayerNumber = data.data.sessionMasterPlayerNumber ? data.data.sessionMasterPlayerNumber : null;
          this.Password = data.data.password;
          this.Players = data.data.players;
          _.forEach(this.Players, player => {
            this.setDefaultNewPlayerName(player);
          });
          this.Loading = false;
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch((err) => this.handleGenericError(err));
  }


  private setDefaultNewPlayerName(player: PlayerDetailsResponse) {
    if (!player.playerName) {
      player.playerName = "New Player";
    }
  }
}
