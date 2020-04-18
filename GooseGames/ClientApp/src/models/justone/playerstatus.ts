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
  ActivePlayerOutcome,

  Rejoining = -1
}
//These should be the same as the required ActionName in PlayerStatusController
//Also these should be the same as the PlayerStatus values in PlayerStatusEnum in the Entities solution


export class PlayerStatusRoutesMaps {
  New: string = "justone/newplayer";
  InLobby: string = "justone/sessionlobby";
  RoundWaiting: string = "justone/round/waitingforround"

  PassivePlayerClue: string = "justone/round/submitclue";
  PassivePlayerWaitingForClues: string = "justone/round/passiveplayerwaiting";
  PassivePlayerClueVote: string = "justone/round/cluevote";
  PassivePlayerWaitingForClueVotes: string = "justone/round/passiveplayerwaitingforvotes";
  PassivePlayerWaitingForActivePlayer: string = "justone/round/passiveplayerwaitingforactiveplayer";
  PassivePlayerOutcome: string = "justone/round/passiveplayeroutcome";
  PassivePlayerOutcomeVote: string = "justone/round/passiveplayeroutcomevote";
  PassivePlayerWaitingForOutcomeVotes: string = "justone/round/passiveplayerwaitingforoutcomevotes";

  ActivePlayerWaitingForClues: string = "justone/round/playerwaiting";
  ActivePlayerWaitingForVotes: string = "justone/round/activeplayerwaitingforcluevotes";
  ActivePlayerGuess: string = "justone/round/activeplayerguess";
  ActivePlayerWaitingForOutcomeVotes: string = "justone/round/activeplayerwaitingforoutcomevotes";
  ActivePlayerOutcome: string = "justone/round/activeplayeroutcome";
};
export const PlayerStatusRoutesMap = new PlayerStatusRoutesMaps();

export class PlayerStatusValidationResponse {
  statusCorrect: boolean;
  requiredStatus: string;
}

