import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { SessionLandingRequest, SessionLandingResponse } from '../../models/justone/session';

@Injectable({
  providedIn: 'root',
})

export class JustOneSessionService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public CreateGame(request: SessionLandingRequest): Promise<GenericResponse<SessionLandingResponse>>
  {
    return this._http.post<GenericResponse<SessionLandingResponse>>(this._baseUrl + 'JustOneSession', request).toPromise();
  }

  public JoinGame(request: SessionLandingRequest): Promise<GenericResponse<SessionLandingResponse>> {
    return this._http.patch<GenericResponse<SessionLandingResponse>>(this._baseUrl + 'JustOneSession', request).toPromise();
  }
}
