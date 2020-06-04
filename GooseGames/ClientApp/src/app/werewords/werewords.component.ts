import { Component, OnInit, ViewChild, ComponentFactoryResolver, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsContent, WerewordsContentDirective } from './scaffolding/content';
import { ActivatedRoute, Router } from '@angular/router';
import { WerewordsPlayerStatusService } from '../../services/werewords/playerstatus';
import * as signalR from "@microsoft/signalr";
import { NavbarService } from '../../services/navbar';
import { WerewordsLandingComponent } from './lobby/landing';
import { WerewordsSessionService } from '../../services/werewords/session';
import { WerewordsPlayerStatus, IWerewordsComponentBase, IWerewordsComponent } from '../../models/werewords/content';
import { RegisteredContent } from './registered-content';

@Component({
  selector: 'app-werewords-component',
  templateUrl: './werewords.component.html',
  styleUrls: ['./werewords.css']
})
export class WerewordsComponent implements OnInit, OnDestroy {

  @ViewChild(WerewordsContentDirective, { static: true }) contentHost: WerewordsContentDirective;
  ComponentSet: boolean;
  PlayerId: string;
  SessionId: string;
  GameIdentifier: string;
  ErrorMessage: string;

  _hubConnection: signalR.HubConnection;

  ngOnInit(): void {

    if (this.SessionId && this.PlayerId) {
      this.setupConnection()
        .then(() => {
          this.route(WerewordsPlayerStatus.InLobby);
        });
    }
    else {
      if (!this.GameIdentifier) {
        this.setContent(new WerewordsContent(null, WerewordsLandingComponent), null);
      }
      else {
        this._sessionService.EnterGame({ password: this.GameIdentifier })
          .then(response => {
            if (response.success) {
              this.SetSessionData(response.data.sessionId, response.data.playerId, this.GameIdentifier);

              this.setupConnection().then(() => {
                this.route(WerewordsPlayerStatus.InLobby);
              });
            }
            else {
              this.router.navigate(['/werewords'], { replaceUrl: true });
            }
          })
          .catch(data => this.HandleGenericError(data));
      }
    }

    this.navbarService.setAreaTitle("Werewords");
  }

  ngOnDestroy(): void {
    if (this._hubConnection) {
      var oldConnection = this._hubConnection;
      oldConnection.onclose(() => { });
      oldConnection.stop();
      this._hubConnection = null;
    }
  }

  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    private playerStatusService: WerewordsPlayerStatusService,
    private _sessionService: WerewordsSessionService,
    private navbarService: NavbarService,
    activatedRoute: ActivatedRoute,
    private router: Router) {

    this.GameIdentifier = activatedRoute.snapshot.params.id;
    if (this.GameIdentifier) {
      this.GameIdentifier = this.GameIdentifier.toLowerCase()
    }
    var urlSessionId = activatedRoute.snapshot.params.SessionId;
    var urlPlayerId = activatedRoute.snapshot.params.PlayerId;

    if (this.GameIdentifier && urlSessionId && urlPlayerId) {

      this.SetSessionData(urlSessionId, urlPlayerId, this.GameIdentifier);

      this.router.navigate(['/werewords', this.GameIdentifier]);
    }
    else if (this.GameIdentifier) {

      var gameIdentifier = this.GameIdentifier;

      this.ReadSessionData(gameIdentifier);
    }
  }

  route = (status: WerewordsPlayerStatus) => {
    var content = _.find(RegisteredContent, c => c.Key === status);

    this.playerStatusService.Validate(<IWerewordsComponentBase>{
      HandleGenericError: this.HandleGenericError,
      SetErrorMessage: this.SetErrorMessage,
      PlayerId: this.PlayerId,
      SessionId: this.SessionId,
      router: this.routeSafe
    }, content.Key, () => { })
      .then(response => {
        if (response.success) {
          this.setContent(content, status);
        }
        else
          if (response.errorCode == "511c0fb3-7d49-4fdf-a1a7-b1281b5ada4b") {
            //Has been previously booted from session, attempt rejoin.

            if (this.GameIdentifier) {
              this._sessionService.EnterGame({ password: this.GameIdentifier })
                .then(response => {
                  if (response.success) {
                    this.SetSessionData(response.data.sessionId, response.data.playerId, this.GameIdentifier);

                    this.setupConnection().then(() => {
                      this.route(WerewordsPlayerStatus.InLobby);
                    });
                  }
                  else {
                    this.router.navigate(['/werewords'], { replaceUrl: true });
                  }
                })
                .catch(data => this.HandleGenericError(data));
            }
          }
          else if (response.errorCode == "a530d7fa-f842-492b-a0fc-6473af1c907a") {
            this.SetSessionData(null, null, null);
            this.router.navigate(['/werewords', this.GameIdentifier], { replaceUrl: true });
          }

      });
  }


  routeSafe = (status: WerewordsPlayerStatus) => {
    var content = _.find(RegisteredContent, c => c.Key === status);

    this.setContent(content, status);
  }

  SetSessionData = (sessionId: string, playerId: string, gameIdentifier: string) => {


    if (gameIdentifier) {
      this.GameIdentifier = gameIdentifier.toLowerCase();
    }

    if (this.GameIdentifier && (!sessionId || !playerId)) {
      localStorage.removeItem(`werewords_${this.GameIdentifier}_Session`);
      localStorage.removeItem(`werewords_${this.GameIdentifier}_Player`);
      localStorage.removeItem(`werewords_${this.GameIdentifier}_Timeout`);
    }
    else {
      this.SessionId = sessionId.toLowerCase();
      this.PlayerId = playerId.toLowerCase();

      if (this.GameIdentifier) {
        localStorage.setItem(`werewords_${this.GameIdentifier}_Session`, this.SessionId);
        localStorage.setItem(`werewords_${this.GameIdentifier}_Player`, this.PlayerId);
        const numberOfMillisecondsPerHour = 3600000;
        var timeout = (new Date().getTime() + numberOfMillisecondsPerHour).toString();
        localStorage.setItem(`werewords_${this.GameIdentifier}_Timeout`, timeout);
      }
    }
  }

  ReadSessionData = (gameIdentifier: string): boolean => {
    const numberOfMillisecondsPerHour = 3600000;
    var timeout = (new Date().getTime() + numberOfMillisecondsPerHour).toString();
    var expiry = localStorage.getItem(`werewords_${gameIdentifier}_Timeout`);
    if (expiry && Number(expiry) > new Date().getTime()) {
      this.SessionId = localStorage.getItem(`werewords_${gameIdentifier}_Session`);
      this.PlayerId = localStorage.getItem(`werewords_${gameIdentifier}_Player`);

      if (this.SessionId && this.PlayerId) {
        const numberOfMillisecondsPerHour = 3600000;
        var timeout = (new Date().getTime() + numberOfMillisecondsPerHour).toString();
        localStorage.setItem(`werewords_${gameIdentifier}_Timeout`, timeout);

        return true;
      }
    }
    return false;
  }

  setContent(werewordsContent: WerewordsContent, status: WerewordsPlayerStatus) {

    const viewContainerRef = this.contentHost.viewContainerRef;
    viewContainerRef.clear();

    if (werewordsContent == null || werewordsContent == undefined) {
      this.ComponentSet = false;
    }
    else {
      const componentFactory = werewordsContent ? this.componentFactoryResolver.resolveComponentFactory(werewordsContent.component) : null;

      if (componentFactory) {

        this.ComponentSet = true;

        const componentRef = viewContainerRef.createComponent(componentFactory);
        (<IWerewordsComponent>componentRef.instance).router = (status: WerewordsPlayerStatus, validated: boolean) => {
          if (!!validated) {
            this.routeSafe(status);
          }
          else {
            this.route(status);
          }
        };
        (<IWerewordsComponent>componentRef.instance).SessionId = this.SessionId;
        (<IWerewordsComponent>componentRef.instance).PlayerId = this.PlayerId;
        (<IWerewordsComponent>componentRef.instance).CurrentStatus = status;
        (<IWerewordsComponent>componentRef.instance).HubConnection = this._hubConnection;
        (<IWerewordsComponent>componentRef.instance).SetSessionData = this.SetSessionData;
        (<IWerewordsComponent>componentRef.instance).ReadSessionData = this.ReadSessionData;
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
      .withUrl(`/werewordshub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
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

  SetErrorMessage = (err: string) => {
    this.ErrorMessage = err;
  }

  HandleGenericError = (err: any) => {
    console.error(err);
    this.SetErrorMessage("Unexpected Error");
  }
}
