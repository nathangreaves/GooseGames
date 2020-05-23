export class PlayerDetails {
  playerName: string;
}

export class UpdatePlayerDetailsRequest extends PlayerDetails {
  playerId: string;
  sessionId: string;
}

export class UpdatePlayerDetailsResponse {

}

export class GetPlayerDetailsRequest {
  sessionId: string;
  playerId: string;
}

export class DeletePlayerRequest {
  sessionMasterId: string;
  playerToDeleteId: string;
}

export class PlayerDetailsResponse {
  id: string;
  playerName: string;
  playerNumber: number;
  isSessionMaster: boolean;
  ready: boolean;
}

export class GetPlayerDetailsResponse {
  sessionMaster: boolean;
  sessionMasterName: string;
  sessionMasterPlayerNumber: number;
  password: string;
  players: PlayerDetailsResponse[]
}

export class PlayerStatusValidationResponse {
  statusCorrect: boolean;
  requiredStatus: string;
}

export class PlayerAction {
  id: string;
  playerName: string;
  playerNumber: number;
  hasTakenAction: boolean;
}
