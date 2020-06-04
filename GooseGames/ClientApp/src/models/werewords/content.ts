
import { IPlayerSession } from '../../models/session';
import { GenericResponseBase, GenericResponse } from '../../models/genericresponse';
import { WerewordsRouter, SetSessionData, ReadSessionData, GenericResponseHandler, GenericResponseBaseHandler } from '../../app/werewords/scaffolding/content';


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
  SetSessionData: SetSessionData;
  ReadSessionData: ReadSessionData;
  //To hook this up, inherit from this interface, create a new enum value in WerewordsContentEnum
  //Then register your new component in module entryComponents in app.module.ts
  //Then add it to the collection RegisteredContent above
}

export class WerewordsComponentBase implements IWerewordsComponent {
  ErrorMessage: string;
  Loading: boolean = true;
  PlayerId: string;
  SessionId: string;
  router: WerewordsRouter;
  CurrentStatus: WerewordsPlayerStatus;
  HubConnection: signalR.HubConnection;
  SetSessionData: SetSessionData;
  ReadSessionData: ReadSessionData;

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
