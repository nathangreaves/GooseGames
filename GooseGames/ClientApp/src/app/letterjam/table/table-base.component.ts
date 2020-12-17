import { GenericResponse, GenericResponseBase } from "../../../models/genericresponse";
import { GenericResponseHandler, GenericResponseBaseHandler } from "../../werewords/scaffolding/content";
import { IPlayerSessionGame } from "../../../models/session";
import { IGetPlayersFromCacheRequest } from "../../../models/letterjam/content";
import { IGooseGamesPlayer } from "../../../models/player";
import { ILetterCard } from "../../../models/letterjam/letters";
import { ICardsRequest } from "../../../models/letterjam/table";

export interface ITableComponentParameters {
  request: IPlayerSessionGame
  getPlayersFromCache: (request: IGetPlayersFromCacheRequest) => Promise<IGooseGamesPlayer[]>;
  getCardsFromCache: (request: ICardsRequest) => Promise<ILetterCard[]>;
  getCurrentRoundId: () => string;
  setCurrentRoundId: (currentRoundId: string) => void;
  hubConnection: signalR.HubConnection;
  setErrorMessage: (errorMessage: string) => void;
  handleGenericError: (err: any) => void;
  handleGenericResponse: <T>(genericResponse: GenericResponse<T>, handleSuccess: GenericResponseHandler<T>) => GenericResponseBase | Promise<GenericResponseBase>;
  handleGenericResponseBase: (genericResponse: GenericResponseBase, handleSuccess: GenericResponseBaseHandler) => GenericResponseBase | Promise<GenericResponseBase>;
}

export class TableComponentBase {
  Loading: boolean;
}
