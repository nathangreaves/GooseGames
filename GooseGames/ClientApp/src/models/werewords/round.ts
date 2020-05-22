import { PlayerRoundInformation } from "./playerroundinformation";

export class DayResponse {
  roundId: string;
  mayorPlayerId: string;
  mayorName: string;
  secretWord: string;
  isActive: boolean;
  players: PlayerRoundInformation[];
  endTime: string
  voteEndTime: string
}
