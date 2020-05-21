import { Type } from '@angular/core';
import { Directive, ViewContainerRef } from '@angular/core';
import { IPlayerSession } from '../session';
import { GenericResponse, GenericResponseBase } from '../genericresponse';

@Directive({
  selector: '[werewords-content]',
})
export class WerewordsContentDirective {
  constructor(public viewContainerRef: ViewContainerRef) { }
}

export class WerewordsContent {
  constructor(public Key: WerewordsPlayerStatus, public component: Type<any>) { }
}

export enum WerewordsPlayerStatus {
  New,
  InLobby,
  RoundWaiting,
  NightRevealSecretRole,
  NightWaitingForPlayersToCheckRole,
  NightMayorPickSecretWord,
  NightWaitingForMayor,
  NightSecretWord,
  NightWaitingToWake,
  DayMayor,
  DayPassive,
  DayActive,
  DayWaitingForVotes,
  DayVotingOnWerewolves,
  DayVotingOnSeer,
  DayOutcome,
  WaitingForNextRound,
  Rejoining = -1
}

type WerewordsRouter = (status: WerewordsPlayerStatus, validated: boolean) => void;

export interface IWerewordsComponentBase extends IPlayerSession {

  router: WerewordsRouter
  SetErrorMessage(err: string);
  HandleGenericError(err: any);
  //To hook this up, inherit from this interface, create a new enum value in WerewordsContentEnum
  //Then register your new component in module entryComponents in app.module.ts
  //Then add it to the collection RegisteredContent above
}

export interface IWerewordsComponent extends IWerewordsComponentBase {

  router: WerewordsRouter
  SetErrorMessage(err: string);
  HandleGenericError(err: any);
  CurrentStatus: WerewordsPlayerStatus;
  HubConnection: signalR.HubConnection;
  //To hook this up, inherit from this interface, create a new enum value in WerewordsContentEnum
  //Then register your new component in module entryComponents in app.module.ts
  //Then add it to the collection RegisteredContent above
}

type GenericResponseHandler<T> = (data: T) => Promise<GenericResponseBase>;
type GenericResponseBaseHandler = () => Promise<GenericResponseBase>;

export class WerewordsComponentBase implements IWerewordsComponent {
  ErrorMessage: string;
  Loading: boolean = true;
  PlayerId: string;
  SessionId: string;
  router: WerewordsRouter;
  CurrentStatus: WerewordsPlayerStatus;
  HubConnection: signalR.HubConnection;

  Route(status: WerewordsPlayerStatus) {
    this.router(status, false);
  }

  RouteToValidated(status: WerewordsPlayerStatus) {
    this.router(status, true);
  }
  
  HandleGenericResponse<T>(genericResponse: GenericResponse<T>, handleSuccess: GenericResponseHandler<T>) {
    if (genericResponse.success) {
      handleSuccess(genericResponse.data);
    }
    else {
      this.SetErrorMessage(genericResponse.errorCode);
    }
    return genericResponse;
  }

  HandleGenericResponseBase(genericResponse: GenericResponseBase, handleSuccess: GenericResponseBaseHandler) {
    if (genericResponse.success) {
      handleSuccess();
    }
    else {
      this.SetErrorMessage(genericResponse.errorCode);
    }
    return genericResponse;
  }

  SetErrorMessage = (err: string) =>
  {
    this.ErrorMessage = err;
  }

  HandleGenericError = (err: any) =>
  {
    console.error(err);
    this.SetErrorMessage("Unexpected Error");
  }
}
