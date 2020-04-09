export class PlayerAction {
  id: string;
  playerName: string;
  playerNumber: number;
  hasTakenAction: boolean;
}

export class PassivePlayerRoundInformationResponse {
  activePlayerName: string;
  activePlayerNumber: number;
  word: string;
}
