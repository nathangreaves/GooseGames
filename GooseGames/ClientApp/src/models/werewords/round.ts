import { PlayerRoundInformation, RoundOutcomePlayerInformation } from "./playerroundinformation";

export class DayResponse {
  roundId: string;
  mayorPlayerId: string;
  mayorName: string;
  secretWord: string;
  isActive: boolean;
  players: PlayerRoundInformation[];
  endTime: string
  voteEndTime: string
  wayOffSpent: boolean;
  soCloseSpent: boolean;
}

export class RoundOutcomeResponse {
  roundOutcome: RoundOutcomeEnum;
  secretWord: string;
  players: RoundOutcomePlayerInformation[];
}

export enum RoundOutcomeEnum {
  VillagersVotedWerewolf = 1,
  VillagersVotedWrong = 2,
  WerewolvesVotedSeer = 3,
  WerewolvesVotedWrong = 4
}
