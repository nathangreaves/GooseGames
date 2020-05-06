import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { SessionLandingRequest, SessionLandingResponse, IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';
import { FujiSession } from '../../models/fujiflush/session';

@Injectable({
  providedIn: 'root',
})

export class FujiSessionService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public CreateGame(request: SessionLandingRequest): Promise<GenericResponse<SessionLandingResponse>>
  {
    return this._http.post<GenericResponse<SessionLandingResponse>>(this._baseUrl + 'FujiSession', request).toPromise();
  }

  public JoinGame(request: SessionLandingRequest): Promise<GenericResponse<SessionLandingResponse>> {
    return this._http.patch<GenericResponse<SessionLandingResponse>>(this._baseUrl + 'FujiSession', request).toPromise();
  }

  public StartSession(request: IPlayerSession): Promise<GenericResponse<boolean>> {

    var request = <IPlayerSession>{
      SessionId: request.SessionId,
      PlayerId: request.PlayerId
    }
    return this._http.post<GenericResponse<boolean>>(this._baseUrl + 'FujiSession/startsession', request).toPromise();
  }

  public GetSession(request: IPlayerSession): Promise<GenericResponse<FujiSession>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<FujiSession>>(this._baseUrl + `FujiSession?${parameters}`).toPromise();
  }

  public CreateTestSession(): Promise<GenericResponse<SessionLandingResponse[]>> {
    return this._http.post<GenericResponse<SessionLandingResponse[]>>(this._baseUrl + 'FujiSession/createtestsession', {}).toPromise();
  }
}
