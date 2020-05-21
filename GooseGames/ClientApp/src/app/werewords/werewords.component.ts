import { Component, OnInit, ViewChild, ComponentFactoryResolver } from '@angular/core';
import * as _ from 'lodash';
import { IWerewordsComponent, WerewordsContent, WerewordsContentDirective, WerewordsPlayerStatus, WerewordsComponentBase, IWerewordsComponentBase } from '../../models/werewords/content';
import { RegisteredContent } from '../../models/werewords/registered-content';
import { ActivatedRoute, Router } from '@angular/router';
import { WerewordsPlayerStatusService } from '../../services/werewords/playerstatus';
import * as signalR from "@microsoft/signalr";

@Component({
  selector: 'app-werewords-component',
  templateUrl: './werewords.component.html'
})
export class WerewordsComponent implements OnInit {

  @ViewChild(WerewordsContentDirective, { static: true }) contentHost: WerewordsContentDirective;
  ComponentSet: boolean;
  PlayerId: string;
  SessionId: string;
  GameIdentifier: string;
  ErrorMessage: string;

  _hubConnection: signalR.HubConnection;

  ngOnInit(): void {

    //this.setContent(contentComponent);

    if (this.SessionId && this.PlayerId) {
      this.setupConnection()
        .then(() => {
          this.route(WerewordsPlayerStatus.NightRevealSecretRole);
        });
    }
    else {
      //TODO: Create or join session

    }

  }


  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    private playerStatusService: WerewordsPlayerStatusService,
    activatedRoute: ActivatedRoute,
    router: Router) {

    this.GameIdentifier = activatedRoute.snapshot.params.id.toLowerCase();
    var urlSessionId = activatedRoute.snapshot.params.SessionId;
    var urlPlayerId = activatedRoute.snapshot.params.PlayerId;

    if (urlSessionId && urlPlayerId) {
      this.SessionId = urlSessionId.toLowerCase();
      this.PlayerId = urlPlayerId.toLowerCase();

      localStorage.setItem(`werewords_${this.GameIdentifier}_Session`, this.SessionId);
      localStorage.setItem(`werewords_${this.GameIdentifier}_Player`, this.PlayerId);
      const numberOfMillisecondsPerHour = 3600000;
      var timeout = (new Date().getTime() + numberOfMillisecondsPerHour).toString();
      localStorage.setItem(`werewords_${this.GameIdentifier}_Timeout`, timeout);

      router.navigate(['/werewords', this.GameIdentifier]);
    }
    else {
      const numberOfMillisecondsPerHour = 3600000;
      var timeout = (new Date().getTime() + numberOfMillisecondsPerHour).toString();
      var expiry = localStorage.getItem(`werewords_${this.GameIdentifier}_Timeout`);

      if (expiry && Number(expiry) > new Date().getTime()) {

        this.SessionId = localStorage.getItem(`werewords_${this.GameIdentifier}_Session`);
        this.PlayerId = localStorage.getItem(`werewords_${this.GameIdentifier}_Player`);

        const numberOfMillisecondsPerHour = 3600000;
        var timeout = (new Date().getTime() + numberOfMillisecondsPerHour).toString();
        localStorage.setItem(`werewords_${this.GameIdentifier}_Timeout`, timeout);
      }
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
      });
  }


  routeSafe = (status: WerewordsPlayerStatus) => {
    var content = _.find(RegisteredContent, c => c.Key === status);

    this.setContent(content, status);
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
      }
    }


  }

  private setupConnection(): Promise<any> {
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
