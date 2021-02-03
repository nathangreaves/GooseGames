import { Component, OnInit, ViewChild, ComponentFactoryResolver } from '@angular/core';
import { AvalonContentDirective, AvalonContent } from './scaffolding/content';
import { IAvalonComponent, AvalonPlayerStatus, IAvalonComponentBase, IGetPlayersFromCacheRequest } from '../../models/avalon/content';
import { GlobalSessionService } from '../../services/session';
import { NavbarService } from '../../services/navbar';
import { ActivatedRoute, Router } from '@angular/router';
import { GooseGamesLocalStorage } from '../../services/localstorage';
//import { AvalonLandingComponent } from './landing/landing.component';
import { RegisteredContent } from './scaffolding/registered-content';
import _ from 'lodash';
import * as signalR from '@microsoft/signalr';
//import { AvalonPlayerStatusService } from '../../services/avalon/playerStatus';
import { IGooseGamesPlayer } from '../../models/player';
import { GlobalPlayerService } from '../../services/player';
import { AvalonLandingComponent } from './landing/landing.component';
import { AvalonSessionService } from '../../services/avalon/session';

@Component({
  selector: 'app-avalon',
  templateUrl: './avalon.component.html',
  styleUrls: ['./avalon.component.scss']
})
export class AvalonComponent implements OnInit {


  @ViewChild(AvalonContentDirective, { static: true }) contentHost: AvalonContentDirective;
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
          this.route(AvalonPlayerStatus.InLobby);
        });
    }
    else {
      if (!this.GamePassword) {
        this.setContent(new AvalonContent(null, AvalonLandingComponent), null);
      }
      else {
        this._sessionService.enterGame({ password: this.GamePassword })
          .then(response => {
            if (response.success) {
              this.SetSessionData(response.data.sessionId, response.data.playerId, this.GamePassword);

              this.setupConnection().then(() => {
                this.route(AvalonPlayerStatus.InLobby);
              });
            }
            else {
              this.router.navigate(['/avalon'], { replaceUrl: true });
            }
          })
          .catch(data => this.HandleGenericError(data));
      }
    }

    this.navbarService.setAreaTitle("Avalon");
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
    private playerStatusService: AvalonSessionService,
    private _sessionService: GlobalSessionService,
    private navbarService: NavbarService,
    activatedRoute: ActivatedRoute,
    private router: Router,
    private globalStorage: GooseGamesLocalStorage,
    private _playerService: GlobalPlayerService) {

    this.GamePassword = activatedRoute.snapshot.params.id;
    if (this.GamePassword) {
      this.GamePassword = this.GamePassword.toLowerCase()
    }
    var urlSessionId = activatedRoute.snapshot.params.SessionId;
    var urlPlayerId = activatedRoute.snapshot.params.PlayerId;

    if (this.GamePassword && urlSessionId && urlPlayerId) {

      this.SetSessionData(urlSessionId, urlPlayerId, this.GamePassword);

      this.router.navigate(['/avalon', this.GamePassword]);
    }
    else if (this.GamePassword) {
      this.ReadSessionData(this.GamePassword);
    }
  }

  route = (status: AvalonPlayerStatus) => {
    var content = _.find(RegisteredContent, c => c.Key === status);

    this.playerStatusService.Validate(<IAvalonComponentBase>{
      HandleGenericError: this.HandleGenericError,
      SetErrorMessage: this.SetErrorMessage,
      PlayerId: this.PlayerId,
      SessionId: this.SessionId,
      GameId: this.GameId,
      router: this.routeSafe,
      SetGameId: this.SetGameId
    }, () => { })
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
                      this.route(AvalonPlayerStatus.InLobby);
                    });
                  }
                  else {
                    this.router.navigate(['/avalon'], { replaceUrl: true });
                  }
                })
                .catch(data => this.HandleGenericError(data));
            }
          }
          else if (response.errorCode == "a530d7fa-f842-492b-a0fc-6473af1c907a") {
            //Player id doesn't exist so clear it from local storage
            this.globalStorage.ClearPlayerDetails();
            this.router.navigate(['/avalon', this.GamePassword], { replaceUrl: true });
          }
      });
  }


  routeSafe = (status: AvalonPlayerStatus) => {
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
    localStorage.setItem('goose-games-avalon-pw', gamePassword);
  }

  ReadSessionData = (gamePassword: string): boolean => {

    var playerDetails = this.globalStorage.GetPlayerDetails();
    if (playerDetails && playerDetails.SessionId && playerDetails.PlayerId) {
      this.SessionId = playerDetails.SessionId;
      this.PlayerId = playerDetails.PlayerId;

      var cachedGameId = localStorage.getItem('goose-games-avalon-pw') as string;
      if (cachedGameId && gamePassword && cachedGameId.toLowerCase() == gamePassword.toLowerCase()) {
        return true;
      }
    }

    return false;
  }

  setContent(avalonContent: AvalonContent, status: AvalonPlayerStatus) {

    const viewContainerRef = this.contentHost.viewContainerRef;
    viewContainerRef.clear();

    if (avalonContent == null || avalonContent == undefined) {
      this.ComponentSet = false;
    }
    else {
      const componentFactory = avalonContent ? this.componentFactoryResolver.resolveComponentFactory(avalonContent.component) : null;

      if (componentFactory) {

        this.ComponentSet = true;

        const componentRef = viewContainerRef.createComponent(componentFactory);
        (<IAvalonComponent>componentRef.instance).router = (status: AvalonPlayerStatus, validated: boolean) => {
          if (!!validated) {
            this.routeSafe(status);
          }
          else {
            this.route(status);
          }
        };
        (<IAvalonComponent>componentRef.instance).SessionId = this.SessionId;
        (<IAvalonComponent>componentRef.instance).PlayerId = this.PlayerId;
        (<IAvalonComponent>componentRef.instance).GameId = this.GameId;
        (<IAvalonComponent>componentRef.instance).CurrentStatus = status;
        (<IAvalonComponent>componentRef.instance).HubConnection = this._hubConnection;
        (<IAvalonComponent>componentRef.instance).SetSessionData = this.SetSessionData;
        (<IAvalonComponent>componentRef.instance).ReadSessionData = this.ReadSessionData;
        (<IAvalonComponent>componentRef.instance).SetGameId = this.SetGameId;
        (<IAvalonComponent>componentRef.instance).GetPlayersFromCache = this.GetPlayersFromCache;
        (<IAvalonComponent>componentRef.instance).RefreshCache = this.RefreshCache;
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
      .withUrl(`/avalonhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
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

    var refreshRequired = firstEntry && this.PlayerCache === null;
    if (refreshRequired) {
      return this.RefreshCache().then(() => this.GetPlayersFromCache(request, false));
    }

    var response = [];

    if (request.allPlayers) {
      return Promise.resolve(this.PlayerCache);
    }

    if (request.playerIds === null) {
      throw new Error("Must request either all players, or specify playerIds to find");
    }

    _.each(request.playerIds, p => {
      var foundPlayer = _.find(this.PlayerCache, pC => pC.id === p);
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
    this.NPCCache = [];

    return Promise.all([this.getPlayers()]);
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
}
