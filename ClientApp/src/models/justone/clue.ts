import { PlayerSessionRequest } from "./session";

export class SubmitClueRequest {
  PlayerId: string;
  SessionId: string
  ResponseWord: string;
}

export class SubmitClueVotesRequest extends PlayerSessionRequest {
   ValidResponses: string[]
}

export class PlayerClue {
  responseId: string;
  playerId: string;
  playerNumber: number;
  playerName: string;
  response: string;
  responseInvalid: boolean;
  responseAutoInvalid: boolean;
}
