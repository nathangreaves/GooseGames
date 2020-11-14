import { Component, OnInit, ViewChild, ComponentFactoryResolver } from '@angular/core';
import { LetterJamContentDirective, LetterJamContent } from './scaffolding/content';
import { ILetterJamComponent, LetterJamPlayerStatus, ILetterJamComponentBase, IGetPlayersFromCacheRequest } from '../../models/letterjam/content';
import { GlobalSessionService } from '../../services/session';
import { NavbarService } from '../../services/navbar';
import { ActivatedRoute, Router } from '@angular/router';
import { GooseGamesLocalStorage } from '../../services/localstorage';
import { LetterJamLandingComponent } from './landing/landing.component';
import { RegisteredContent } from './scaffolding/registered-content';
import _ from 'lodash';
import * as signalR from '@microsoft/signalr';
import { LetterJamPlayerStatusService } from '../../services/letterjam/playerStatus';
import { IGooseGamesPlayer } from '../../models/player';
import { GlobalPlayerService } from '../../services/player';
import { LetterJamTableService } from '../../services/letterjam/table';

@Component({
  selector: 'app-letter-jam',
  templateUrl: './letter-jam.component.html',
  styleUrls: ['./letter-jam.component.scss']
})
export class LetterJamComponent implements OnInit {


  @ViewChild(LetterJamContentDirective, { static: true }) contentHost: LetterJamContentDirective;
  ComponentSet: boolean;
  PlayerId: string;
  SessionId: string;
  GameId: string;
  GamePassword: string;
  ErrorMessage: string;

  _hubConnection: signalR.HubConnection;

  ngOnInit(): void {

    if (this.ReadSessionData(this.GamePassword)) {
      this.setupConnection()
        .then(() => {
          this.route(LetterJamPlayerStatus.InLobby);
        });
    }
    else {
      if (!this.GamePassword) {
        this.setContent(new LetterJamContent(null, LetterJamLandingComponent), null);
      }
      else {
        this._sessionService.enterGame({ password: this.GamePassword })
          .then(response => {
            if (response.success) {
              this.SetSessionData(response.data.sessionId, response.data.playerId, this.GamePassword);

              this.setupConnection().then(() => {
                this.route(LetterJamPlayerStatus.InLobby);
              });
            }
            else {
              this.router.navigate(['/letterjam'], { replaceUrl: true });
            }
          })
          .catch(data => this.HandleGenericError(data));
      }
    }

    this.navbarService.setAreaTitle("LetterJam");
  }

  ngOnDestroy(): void {
    if (this._hubConnection) {
      var oldConnection = this._hubConnection;
      this._hubConnection = null;
      oldConnection.onclose(() => { });
      oldConnection.stop();
    }
  }

  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    private playerStatusService: LetterJamPlayerStatusService,
    private _sessionService: GlobalSessionService,
    private navbarService: NavbarService,
    activatedRoute: ActivatedRoute,
    private router: Router,
    private globalStorage: GooseGamesLocalStorage,
    private _playerService: GlobalPlayerService,
    private _nonPlayerCharacterService: LetterJamTableService) {

    this.GamePassword = activatedRoute.snapshot.params.id;
    if (this.GamePassword) {
      this.GamePassword = this.GamePassword.toLowerCase()
    }
    var urlSessionId = activatedRoute.snapshot.params.SessionId;
    var urlPlayerId = activatedRoute.snapshot.params.PlayerId;

    if (this.GamePassword && urlSessionId && urlPlayerId) {

      this.SetSessionData(urlSessionId, urlPlayerId, this.GamePassword);

      this.router.navigate(['/letterjam', this.GamePassword]);
    }
    else if (this.GamePassword) {
      this.ReadSessionData(this.GamePassword);
    }
  }

  route = (status: LetterJamPlayerStatus) => {
    var content = _.find(RegisteredContent, c => c.Key === status);

    this.playerStatusService.Validate(<ILetterJamComponentBase>{
      HandleGenericError: this.HandleGenericError,
      SetErrorMessage: this.SetErrorMessage,
      PlayerId: this.PlayerId,
      SessionId: this.SessionId,
      GameId: this.GameId,
      router: this.routeSafe,
      SetGameId: this.SetGameId
    }, content.Key, () => { })
      .then(response => {
        if (response.success) {
          this.setContent(content, status);
        }
        else
          if (response.errorCode == "511c0fb3-7d49-4fdf-a1a7-b1281b5ada4b") {
            //Has been previously booted from session, attempt rejoin.

            if (this.GamePassword) {
              this._sessionService.enterGame({ password: this.GamePassword })
                .then(response => {
                  if (response.success) {
                    this.SetSessionData(response.data.sessionId, response.data.playerId, this.GamePassword);

                    this.setupConnection().then(() => {
                      this.route(LetterJamPlayerStatus.InLobby);
                    });
                  }
                  else {
                    this.router.navigate(['/letterjam'], { replaceUrl: true });
                  }
                })
                .catch(data => this.HandleGenericError(data));
            }
          }
          else if (response.errorCode == "a530d7fa-f842-492b-a0fc-6473af1c907a") {
            //Player id doesn't exist so clear it from local storage
            this.globalStorage.ClearPlayerDetails();
            this.router.navigate(['/letterjam', this.GamePassword], { replaceUrl: true });
          }
      });
  }


  routeSafe = (status: LetterJamPlayerStatus) => {
    var content = _.find(RegisteredContent, c => c.Key === status);

    this.setContent(content, status);
  }

  SetSessionData = (sessionId: string, playerId: string, gamePassword: string) => {

    if (gamePassword) {
      this.GamePassword = gamePassword.toLowerCase();
    }
    this.SessionId = sessionId;
    this.PlayerId = playerId;

    this.globalStorage.CachePlayerDetails({ PlayerId: playerId, SessionId: sessionId });
    localStorage.setItem('goose-games-letterjam-pw', gamePassword);
  }

  ReadSessionData = (gamePassword: string): boolean => {

    var playerDetails = this.globalStorage.GetPlayerDetails();
    if (playerDetails && playerDetails.SessionId && playerDetails.PlayerId) {
      this.SessionId = playerDetails.SessionId;
      this.PlayerId = playerDetails.PlayerId;

      var cachedGameId = localStorage.getItem('goose-games-letterjam-pw') as string;
      if (cachedGameId && gamePassword && cachedGameId.toLowerCase() == gamePassword.toLowerCase()) {
        return true;
      }
    }

    return false;
  }

  setContent(letterJamContent: LetterJamContent, status: LetterJamPlayerStatus) {

    const viewContainerRef = this.contentHost.viewContainerRef;
    viewContainerRef.clear();

    if (letterJamContent == null || letterJamContent == undefined) {
      this.ComponentSet = false;
    }
    else {
      const componentFactory = letterJamContent ? this.componentFactoryResolver.resolveComponentFactory(letterJamContent.component) : null;

      if (componentFactory) {

        this.ComponentSet = true;

        const componentRef = viewContainerRef.createComponent(componentFactory);
        (<ILetterJamComponent>componentRef.instance).router = (status: LetterJamPlayerStatus, validated: boolean) => {
          if (!!validated) {
            this.routeSafe(status);
          }
          else {
            this.route(status);
          }
        };
        (<ILetterJamComponent>componentRef.instance).SessionId = this.SessionId;
        (<ILetterJamComponent>componentRef.instance).PlayerId = this.PlayerId;
        (<ILetterJamComponent>componentRef.instance).GameId = this.GameId;
        (<ILetterJamComponent>componentRef.instance).CurrentStatus = status;
        (<ILetterJamComponent>componentRef.instance).HubConnection = this._hubConnection;
        (<ILetterJamComponent>componentRef.instance).SetSessionData = this.SetSessionData;
        (<ILetterJamComponent>componentRef.instance).ReadSessionData = this.ReadSessionData;
        (<ILetterJamComponent>componentRef.instance).SetGameId = this.SetGameId;
        (<ILetterJamComponent>componentRef.instance).GetPlayersFromCache = this.GetPlayersFromCache;
      }
    }


  }

  private setupConnection(): Promise<any> {

    if (this._hubConnection) {
      var oldConnection = this._hubConnection;
      oldConnection.onclose(() => { });
      oldConnection.stop();
    }

    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/letterjamhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {
      //this.Validate();
    });
    this._hubConnection.onclose(() => {
      this.HandleGenericError("connection closed");
    });

    return this._hubConnection.start().catch(err => console.error(err));
  }

  SetGameId = (gameId: string) => {
    if (this.GameId !== gameId) {
      this.GameId = gameId;
    }
  }

  PlayerCache: IGooseGamesPlayer[] = null;
  NPCCache: IGooseGamesPlayer[] = null;

  GetPlayersFromCache = (request: IGetPlayersFromCacheRequest, firstEntry: boolean = true): Promise<IGooseGamesPlayer[]> => {

    var refreshRequired = firstEntry && ((request.includeReal && this.PlayerCache === null) || (request.includeNPC && this.NPCCache.length === null));
    if (refreshRequired) {
      return this.RefreshCache().then(() => this.GetPlayersFromCache(request, false));
    }

    var response = [];

    if (request.allPlayers) {
      return Promise.resolve((request.includeReal ? this.PlayerCache : []).concat(request.includeNPC ? this.NPCCache : []));
    }

    if (request.playerIds === null) {
      throw new Error("Must request either all players, or specify playerIds to find");
    }

    _.each(request.playerIds, p => {
      var foundPlayer = request.includeReal ? _.find(this.PlayerCache, pC => pC.id === p) : null;
      if (!foundPlayer) {
        foundPlayer = request.includeNPC ? _.find(this.NPCCache, pc => pc.id === p) : null;
      }
      if (foundPlayer) {
        response.push(foundPlayer);
      }
      else {
        throw new Error(`Unable to find requested playerid = ${p}`);
      }
    });

    return Promise.resolve(response);

  }

  RefreshCache = (): Promise<any> => {
    this.PlayerCache = [];
    this.NPCCache = []; //TODO: Get npcs

    return Promise.all([this.getPlayers(), this.getNonPlayerCharacters()]);
  }

  SetErrorMessage = (err: string) => {
    this.ErrorMessage = err;
  }

  HandleGenericError = (err: any) => {
    console.error(err);
    this.SetErrorMessage("Unexpected Error");
  }

  private getPlayers(): Promise<any> {
    return this._playerService.getPlayers(this).then(response => {
      if (response.success) {
        _.each(response.data, p => this.PlayerCache.push(p));
      }
      else {
        this.SetErrorMessage(response.errorCode);
      }
    }).catch(this.HandleGenericError);
  }
  private getNonPlayerCharacters(): Promise<any> {
    return this._nonPlayerCharacterService.GetNonPlayerCharacters(this).then(response => {
      if (response.success) {
        _.each(response.data, p => this.NPCCache.push({
          id: p.nonPlayerCharacterId,
          emoji: p.emoji,
          name: p.name,
          playerNumber: p.playerNumber
        }));
      }
      else {
        this.SetErrorMessage(response.errorCode);
      }
    }).catch(this.HandleGenericError);
  }
}
