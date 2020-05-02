import { PlayerSessionRequest } from "../session";

export class PlayerSessionRoundRequest extends PlayerSessionRequest {
  RoundId: string;
}

export enum RoundOutcomeEnum {
  Undetermined = 0,
  Success = 1,
  Pass = 2,
  Fail = 3
}

export class RoundOutcomeResponse {
  gameEnded: boolean;
  score: number;
  wordToGuess: string;
  wordGuessed: string;
  activePlayerName: string;
  activePlayerNumber: number;
  roundOutcome: RoundOutcomeEnum;
  roundId: string;
  nextRoundInformation: RoundInformationResponse;
}

export class RoundInformationResponse {
  roundNumber: number;
  roundsTotal: number;
  score: number;
}
