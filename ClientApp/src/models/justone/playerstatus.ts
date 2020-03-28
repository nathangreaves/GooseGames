export enum PlayerStatus {
  New,
  InLobby,
  RoundWaiting,
  RoundSubmitClue,
  RoundPlayerWaiting
}
//These should be the same as the required ActionName in PlayerStatusController
//Also these should be the same as the PlayerStatus values in PlayerStatusEnum in the Entities solution


export const PlayerStatusRoutesMap: Record<PlayerStatus, string> = {
  [PlayerStatus.InLobby]: "justone/sessionlobby",
  [PlayerStatus.New]: "justone/newplayer",
  [PlayerStatus.RoundWaiting]: "justone/round/waiting",
  [PlayerStatus.RoundSubmitClue]: "justone/round/submitclue",
  [PlayerStatus.RoundPlayerWaiting]: "justone/round/playerwaiting",
};

export class PlayerStatusValidationResponse {
  statusCorrect: boolean;
  requiredStatus: string;
}

