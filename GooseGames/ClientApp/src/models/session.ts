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

export interface IPlayerSessionGame {
  SessionId: string;
  PlayerId: string;
  GameId: string;
}

export function ConvertToPlayerSessionRequest(playerSession: IPlayerSession): IPlayerSession
{
  return {
    SessionId: playerSession.SessionId,
    PlayerId: playerSession.PlayerId
  }  
}

export function ConvertToPlayerSessionGameRequest(playerSession: IPlayerSessionGame): IPlayerSessionGame {
  return {
    SessionId: playerSession.SessionId,
    PlayerId: playerSession.PlayerId,
    GameId: playerSession.GameId
  }
}

export interface IPlayerSessionComponent extends IPlayerSession {
  ErrorMessage: string;
  HandleGenericError();
}

export class PlayerSessionRequest implements IPlayerSession {
  SessionId: string;
  PlayerId: string;
}
