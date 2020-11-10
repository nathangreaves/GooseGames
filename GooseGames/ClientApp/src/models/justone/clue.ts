import { PlayerSessionRequest } from "../session";

export class SubmitClueRequest {
  PlayerId: string;
  SessionId: string
  ResponseWord: string;
}

export class SubmitActivePlayerResponseRequest extends SubmitClueRequest {
  Pass: boolean;
}

export class SubmitClueVotesRequest extends PlayerSessionRequest {
   ValidResponses: string[]
}

export class PlayerCluesResponse {
  activePlayerName: string;
  activePlayerNumber: number;
  activePlayerEmoji: string;
  wordToGuess: string;
  responses: PlayerClue[]
}

export class PlayerClue {
  responseId: string;
  playerId: string;
  playerNumber: number;
  playerName: string;
  playerEmoji: string;
  response: string;
  responseInvalid: boolean;
  responseVoted: boolean;
}
