
import { IPlayerSessionGame, IPlayerSession } from '../../models/session';
import { GenericResponseBase, GenericResponse } from '../../models/genericresponse';
import { AvalonRouter, GenericResponseHandler, GenericResponseBaseHandler, ReadSessionData, SetSessionData } from '../../app/avalon/scaffolding/content';
import { IGooseGamesPlayer } from '../player';
import { IKnownErrorCode } from '../error';
import _ from 'lodash';

export enum AvalonPlayerStatus {
  InLobby,
  InGame
}

export interface IAvalonComponentBase extends IPlayerSession {

  GameId: string;
  router: AvalonRouter;
  SetErrorMessage(err: string);
  HandleGenericError(err: any);
  SetGameId: (gameId: string) => void;
  //To hook this up, inherit from this interface, create a new enum value in WerewordsContentEnum
  //Then register your new component in module entryComponents in app.module.ts
  //Then add it to the collection RegisteredContent above
}

export interface IAvalonComponent extends IAvalonComponentBase {

  router: AvalonRouter;
  SetErrorMessage(err: string);
  HandleGenericError(err: any);
  CurrentStatus: AvalonPlayerStatus;
  HubConnection: signalR.HubConnection;
  SetSessionData: SetSessionData;
  ReadSessionData: ReadSessionData;
  GetPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  RefreshCache: () => void;
  //To hook this up, inherit from this interface, create a new enum value in WerewordsContentEnum
  //Then register your new component in module entryComponents in app.module.ts
  //Then add it to the collection RegisteredContent above
}

export interface IGetPlayersFromCacheRequest {
  playerIds: string[] | null;
  allPlayers: boolean | null;
}

export abstract class PlayersFromCacheRequestBase {
  playerIds: string[] | null = null;
  allPlayers: boolean | null = false;
}

export class PlayersFromCacheRequest extends PlayersFromCacheRequestBase implements IGetPlayersFromCacheRequest {

  constructor(playerIds: string[]) {
    super();
    this.playerIds = playerIds;
  }
}
export class AllPlayersFromCacheRequest extends PlayersFromCacheRequestBase implements IGetPlayersFromCacheRequest {

  constructor() {
    super();
    this.allPlayers = true;
  }
}

export class AvalonComponentBase implements IAvalonComponent {
  ErrorMessage: string;
  Loading: boolean = true;
  PlayerId: string;
  SessionId: string;
  GameId: string;
  router: AvalonRouter;
  CurrentStatus: AvalonPlayerStatus;
  HubConnection: signalR.HubConnection;
  SetSessionData: SetSessionData;
  SetGameId: (gameId: string) => void;
  ReadSessionData: ReadSessionData;
  GetPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  RefreshCache: () => void;
  KnownErrorCodes: IKnownErrorCode[] = [];

  Route(status: AvalonPlayerStatus) {
    this.router(status, false);
  }

  RouteToValidated(status: AvalonPlayerStatus) {
    this.router(status, true);
  }
  
  HandleGenericResponse<T>(genericResponse: GenericResponse<T>, handleSuccess: GenericResponseHandler<T>) {
    if (genericResponse.success) {
      return handleSuccess(genericResponse.data);
    }
    else {
      this.SetErrorMessage(genericResponse.errorCode);
    }
    return genericResponse;
  }

  HandleGenericResponseBase(genericResponse: GenericResponseBase, handleSuccess: GenericResponseBaseHandler) {
    if (genericResponse.success) {
      return handleSuccess();
    }
    else {
      this.SetErrorMessage(genericResponse.errorCode);
    }
    return genericResponse;
  }

  SetErrorMessage = (err: string) =>
  {
    var knownError = _.find(this.KnownErrorCodes, k => k.errorCode === err);
    if (knownError) {
      this.ErrorMessage = knownError.errorMessage;
    }
    else {
      this.ErrorMessage = err;
    }
  }

  HandleGenericError = (err: any) =>
  {
    console.error(err);
    this.SetErrorMessage("Unexpected Error");
  }

  ClearErrorMessage = () => {
    this.ErrorMessage = null;
  }
}
