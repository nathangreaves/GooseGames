export class SessionLandingRequest {
  password: string;
}

export class SessionLandingResponse {
  sessionId: string;
  playerId: string;
}

export interface IPlayerSession {
  SessionId: string;
  PlayerId: string;
}
export interface IPlayerSessionComponent extends IPlayerSession {
  ErrorMessage: string;
  HandleGenericError();
}

export class PlayerSessionRequest {
  sessionId: string;
  playerId: string;
}
