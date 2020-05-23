import { Component, OnDestroy, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { PlayerDetails, UpdatePlayerDetailsRequest, PlayerDetailsResponse } from '../../../models/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { WerewordsComponentBase, WerewordsPlayerStatus } from '../../../models/werewords/content';
import { WerewordsSessionService } from '../../../services/werewords/session';

@Component({
  selector: 'app-werewords-lobby-component',
  templateUrl: './lobby.html',
  styleUrls: ['./lobby.css'],
})

export class WerewordsLobbyComponent extends WerewordsComponentBase implements OnInit, OnDestroy {

  MinPlayers: number = 4;

  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;
  public Players: PlayerDetailsResponse[];


  DisableButtons: boolean;
    StatusText: string;

  constructor(private _sessionService: WerewordsSessionService, router: Router, activatedRoute: ActivatedRoute) {
    super();

    this.Loading = true;

  }
  ngOnDestroy(): void {



    }
  ngOnInit(): void {

    this.HubConnection.on("playerAdded", (player: PlayerDetailsResponse) => {
      this.setDefaultNewPlayerName(player);
      this.Players.push(player);
    });
    this.HubConnection.on("playerDetailsUpdated", (player: PlayerDetailsResponse) => {
      if (this.Players && this.Players.length > 0) {
        var index = _.findIndex(this.Players, p => p.id == player.id);
        this.Players.splice(index, 1, player);
      }
    });
    this.HubConnection.on("playerRemoved", (playerId: string) => {
      if (this.Players && this.Players.length > 0) {
        _.remove(this.Players, p => p.id === playerId);
      }
    });
    this.HubConnection.on("startingSession", () => {
      this.StatusText = "Setting up the game...";
    });
    this.HubConnection.on("secretRole", () => {
      
      this.Route(WerewordsPlayerStatus.NightRevealSecretRole);
    });


    this.load();
    }


  public StartGame() {

    this.ErrorMessage = null;
    if (_.find(this.Players, p => p.playerNumber == 0 || !p.ready)) {
      this.ErrorMessage = "Not all players are ready";
      return;
    }

    this.DisableButtons = true;
    this._sessionService.StartSession(this)
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch((err) => {
        this.HandleGenericError(err);
        this.DisableButtons = false;
      });
  }

  public KickPlayer(playerId: string) {
    if (playerId == this.PlayerId) {
      return;
    }
    this.DisableButtons = true;
    this._sessionService.DeletePlayer({ sessionMasterId: this.PlayerId, playerToDeleteId: playerId })
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch((err) => this.HandleGenericError(err))
      .finally(() => this.DisableButtons = false);
  }

  private load() {
    this._sessionService.GetPlayerDetails(this)
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
      .catch((err) => this.HandleGenericError(err));
  }


  CloseConnection() {
    var connection = this.HubConnection;
    if (connection) {
      connection.off("playerAdded");
      connection.off("playerDetailsUpdated");
      connection.off("playerRemoved");
      connection.off("startingSession");
      connection.off("secretRole");
    }
  }

  private setDefaultNewPlayerName(player: PlayerDetailsResponse) {
    if (!player.playerName) {
      player.playerName = "New Player";
    }
  }
}
