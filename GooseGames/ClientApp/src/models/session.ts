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

export function ConvertToPlayerSessionRequest(playerSession: IPlayerSession): IPlayerSession
{
  return {
    SessionId: playerSession.SessionId,
    PlayerId: playerSession.PlayerId
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
