export enum PlayerStatus {
  New,
  InLobby,
  RoundWaiting,

  PassivePlayerClue,
  PassivePlayerWaitingForClues,
  PassivePlayerClueVote,
  PassivePlayerWaitingForClueVotes,
  PassivePlayerWaitingForActivePlayer,
  PassivePlayerOutcome,
  PassivePlayerOutcomeVote,
  PassivePlayerWaitingForOutcomeVotes,

  ActivePlayerWaitingForClues,
  ActivePlayerWaitingForVotes,
  ActivePlayerGuess,
  ActivePlayerWaitingForOutcomeVotes,
  ActivePlayerOutcome
}
//These should be the same as the required ActionName in PlayerStatusController
//Also these should be the same as the PlayerStatus values in PlayerStatusEnum in the Entities solution


export const PlayerStatusRoutesMap: Record<PlayerStatus, string> = {
  [PlayerStatus.New]: "justone/newplayer",
  [PlayerStatus.InLobby]: "justone/sessionlobby",
  [PlayerStatus.RoundWaiting]: "justone/round/waiting",

  [PlayerStatus.PassivePlayerClue]: "justone/round/submitclue",
  [PlayerStatus.PassivePlayerWaitingForClues]: "justone/round/passiveplayerwaiting",
  [PlayerStatus.PassivePlayerClueVote]: "justone/round/cluevote",
  [PlayerStatus.PassivePlayerWaitingForClueVotes]: "justone/round/passiveplayerwaitingforvotes",
  [PlayerStatus.PassivePlayerWaitingForActivePlayer]: "justone/round/passiveplayerwaitingforactiveplayer",
  [PlayerStatus.PassivePlayerOutcome]: "justone/round/passiveplayeroutcome",
  [PlayerStatus.PassivePlayerOutcomeVote]: "justone/round/passiveplayeroutcomevote",
  [PlayerStatus.PassivePlayerWaitingForOutcomeVotes]: "justone/round/passiveplayerwaitingforoutcomevotes",

  [PlayerStatus.ActivePlayerWaitingForClues]: "justone/round/playerwaiting",
  [PlayerStatus.ActivePlayerWaitingForVotes]: "justone/round/activeplayerwaitingforcluevotes",
  [PlayerStatus.ActivePlayerGuess]: "justone/round/activeplayerguess",
  [PlayerStatus.ActivePlayerWaitingForOutcomeVotes]: "justone/round/activeplayerwaitingforoutcomevotes",
  [PlayerStatus.ActivePlayerOutcome]: "justone/round/activeplayeroutcome"
};

export class PlayerStatusValidationResponse {
  statusCorrect: boolean;
  requiredStatus: string;
}

