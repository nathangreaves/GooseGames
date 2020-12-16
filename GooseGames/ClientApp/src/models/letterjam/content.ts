
import { IPlayerSessionGame, IPlayerSession } from '../../models/session';
import { GenericResponseBase, GenericResponse } from '../../models/genericresponse';
import { LetterJamRouter, GenericResponseHandler, GenericResponseBaseHandler, ReadSessionData, SetSessionData } from '../../app/letterjam/scaffolding/content';
import { IGooseGamesPlayer } from '../player';
import { IKnownErrorCode } from '../error';
import _ from 'lodash';

export enum LetterJamPlayerStatus {
  InLobby,
  ConstructingWord,
  WaitingForFirstRound,
  ProposingClues,
  ReceivedClue,
  ReadyForNextRound,
  PreparingFinalWord,
  SubmittedFinalWord,
  ReadyForGameEnd,
  ReviewingGameEnd
}

export interface ILetterJamComponentBase extends IPlayerSession {

  GameId: string;
  router: LetterJamRouter
  SetErrorMessage(err: string);
  HandleGenericError(err: any);
  SetGameId: (gameId: string) => void;
  //To hook this up, inherit from this interface, create a new enum value in WerewordsContentEnum
  //Then register your new component in module entryComponents in app.module.ts
  //Then add it to the collection RegisteredContent above
}

export interface ILetterJamComponent extends ILetterJamComponentBase {

  router: LetterJamRouter
  SetErrorMessage(err: string);
  HandleGenericError(err: any);
  CurrentStatus: LetterJamPlayerStatus;
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
  includeReal: boolean;
  includeNPC: boolean;
}

export interface ILetterJamRoundRequest extends IPlayerSessionGame {
  roundId: string;
}

export abstract class PlayersFromCacheRequestBase {
  includeReal: boolean;
  includeNPC: boolean;
  playerIds: string[] | null = null;
  allPlayers: boolean | null = false;
  constructor(includeReal, includeNPC) {
    this.includeReal = includeReal;
    this.includeNPC = includeNPC;
  }
}

export class PlayersFromCacheRequest extends PlayersFromCacheRequestBase implements IGetPlayersFromCacheRequest {

  constructor(playerIds: string[], includeReal = true, includeNPC = false) {
    super(includeReal, includeNPC);
    this.playerIds = playerIds;
  }
}
export class AllPlayersFromCacheRequest extends PlayersFromCacheRequestBase implements IGetPlayersFromCacheRequest {

  constructor(includeReal = true, includeNPC = false) {
    super(includeReal, includeNPC);
    this.allPlayers = true;
  }
}

export class LetterJamComponentBase implements ILetterJamComponent {
  ErrorMessage: string;
  Loading: boolean = true;
  PlayerId: string;
  SessionId: string;
  GameId: string;
  router: LetterJamRouter;
  CurrentStatus: LetterJamPlayerStatus;
  HubConnection: signalR.HubConnection;
  SetSessionData: SetSessionData;
  SetGameId: (gameId: string) => void;
  ReadSessionData: ReadSessionData;
  GetPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  RefreshCache: () => void;
  KnownErrorCodes: IKnownErrorCode[] = [];

  Route(status: LetterJamPlayerStatus) {
    this.router(status, false);
  }

  RouteToValidated(status: LetterJamPlayerStatus) {
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
