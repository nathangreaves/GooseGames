import { Component, OnInit, Input, TemplateRef, HostListener, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { PlayerDetailsResponse } from '../../../models/player';
import { GlobalSessionService } from '../../../services/session';
import { GenericResponseBase } from '../../../models/genericresponse';
import { IGlobalLobbyHubParameters } from './hub';
import { GooseGamesLocalStorage } from '../../../services/localstorage';
import { Router } from '@angular/router';
import { EmojiButton } from '@joeattardi/emoji-button';
import * as EmojiRegex from 'emoji-regex/es2015/RGI_Emoji';
import { PlatformLocation } from '@angular/common';

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
export class LobbyComponent implements OnInit, OnDestroy {

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

  PlayerName: string;
  playerReady: boolean;
  selectedEmoji: string;

  globalHubConnectionParameters: IGlobalLobbyHubParameters;

  picker = new EmojiButton({
    emojisPerRow: 6,
    style: 'twemoji',
    autoFocusSearch: false
  });
  emojiRegExp: any;

  constructor(private _globalSessionService: GlobalSessionService, private localStorage: GooseGamesLocalStorage, private router: Router)
  {
    this.Loading = true;

    this.PlayerName = this.localStorage.GetPlayerName();
    this.selectedEmoji = this.localStorage.GetPlayerEmoji();

    this.picker.on('emoji', selection => {
      this.selectedEmoji = selection.emoji;
      this.picker.hidePicker();
    });
    this.picker.on('hidden', this.onHidden);

    this.emojiRegExp = EmojiRegex();
  }

  @HostListener('window:popstate', ['$event'])
  dismissModal() {
    if (this.picker.isPickerVisible()) {
      this.picker.hidePicker();
    }
  }

  onHidden = () => {
    if (window.history.state.modal === true) {
      window.history.state.modal = false;
      history.back();
    }
  }

  loadEmojiPicker(element: any) {
    this.picker.togglePicker(element);

    const modalState = {
      modal: true,
      desc: 'fake state for our modal'
    };
    history.pushState(modalState, null);
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

  playerRemoved = (playerId: string) => {

    if (playerId == this.parameters.playerId) {
      this.router.navigate(['']);
    }
    else if (this.Players && this.Players.length > 0) {
      var index = _.findIndex(this.Players, p => p.id == playerId);
      this.Players.splice(index, 1);
    }
  }

  handleGenericError(err: string) {
    this.ErrorMessage = err;
  }

  ngOnInit(): void {

    var resolveConnected;    
    var loadingPromise = new Promise((resolve, reject) => {
      resolveConnected = resolve;
    });

    this.globalHubConnectionParameters = {
      playerId: this.parameters.playerId,
      sessionId: this.parameters.sessionId,
      handleConnectionError: this.handleGenericError,
      resolveConnected: resolveConnected,
      handleReconnected: () => { },
      handlePlayerAdded: this.playerAdded,
      handlePlayerDetailsUpdated: this.playerDetailsUpdated,
      handlePlayerRemoved: this.playerRemoved
    };

    this.load().then(() => loadingPromise).then(() => {
      this.Loading = false;
    });
  }

  ngOnDestroy() {
    this.picker.destroyPicker();
    this.picker = null;
  }

  public StartGame() {

    this.ErrorMessage = null;
    if (_.find(this.Players, p => !p.ready)) {
      this.ErrorMessage = "Not all players are ready";
      return;
    }

    this.DisableButtons = true;
    this.parameters.startSession(this.parameters.sessionId, this.parameters.playerId)
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
          this.DisableButtons = false;
        }
      })
      .catch((err) => {
        this.handleGenericError(err);
        this.DisableButtons = false;
      });
  }

  public Ready = () => {

    if (this.playerReady) {
      return this._globalSessionService.unreadyPlayer({
        PlayerId: this.parameters.playerId,
        SessionId: this.parameters.sessionId
      }).then(data => {
        if (data.success) {
          this.playerReady = false;
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
        .catch(err => this.handleGenericError(err));
    }

    return this._globalSessionService.updatePlayerDetails({
      playerId: this.parameters.playerId,
      sessionId: this.parameters.sessionId,
      playerName: this.PlayerName,
      emoji: this.selectedEmoji
    })
      .then(data => {
        if (data.success) {
          this.localStorage.CachePlayerDetails({
            PlayerId: this.parameters.playerId,
            SessionId: this.parameters.sessionId,
          }, this.PlayerName, this.selectedEmoji);
          this.playerReady = true;
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch(err => this.handleGenericError(err));
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
    return this._globalSessionService.getPlayerDetails({ PlayerId: this.parameters.playerId, SessionId: this.parameters.sessionId })
      .then(data => {
        if (data.success) {

          this.SessionMaster = data.data.sessionMaster;
          this.SessionMasterName = data.data.sessionMasterName ? data.data.sessionMasterName : "Session Master";
          this.SessionMasterPlayerNumber = data.data.sessionMasterPlayerNumber ? data.data.sessionMasterPlayerNumber : null;
          this.Password = data.data.password;
          this.Players = data.data.players;

          _.forEach(this.Players, player => {

            if (player.id == this.parameters.playerId) {
              this.playerReady = player.ready;
              if (player.playerName) {
                this.PlayerName = player.playerName;
              }
              if (player.emoji) {
                this.selectedEmoji = player.emoji;
              }
            }
            this.setDefaultNewPlayerName(player);
          });

          if (!this.selectedEmoji || !this.isEmoji(this.selectedEmoji)) {
            this.selectedEmoji = data.data.randomEmoji;
          }
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch((err) => this.handleGenericError(err));
  }


    private isEmoji(emoji: string): boolean {
      var result = this.emojiRegExp.exec(this.selectedEmoji);
      return result;
    }

  private setDefaultNewPlayerName(player: PlayerDetailsResponse) {
    if (!player.playerName) {
      player.playerName = "New Player";
    }
  }
}
