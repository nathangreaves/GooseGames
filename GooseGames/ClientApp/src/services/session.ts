import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../models/genericresponse';
import { SessionLandingRequest, SessionLandingResponse, IPlayerSession, ConvertToPlayerSessionRequest } from '../models/session';
import { UpdatePlayerDetailsRequest, UpdatePlayerDetailsResponse, GetPlayerDetailsResponse, DeletePlayerRequest } from '../models/player';

@Injectable({
  providedIn: 'root',
})
export class GlobalSessionService {

  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public enterGame(request: SessionLandingRequest): Promise<GenericResponse<SessionLandingResponse>> {
    return this._http.post<GenericResponse<SessionLandingResponse>>(this._baseUrl + 'GlobalSession', request).toPromise();
  }

  public getPlayerDetails(request: IPlayerSession): Promise<GenericResponse<GetPlayerDetailsResponse>> {

    var request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<GetPlayerDetailsResponse>>(this._baseUrl + 'GlobalPlayer/Lobby?' + parameters).toPromise();
  }

  public deletePlayer(request: DeletePlayerRequest): Promise<GenericResponseBase> {
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.delete<GenericResponseBase>(this._baseUrl + 'GlobalPlayer?' + parameters).toPromise();
  }

  public updatePlayerDetails(request: UpdatePlayerDetailsRequest): Promise<GenericResponseBase> {
    return this._http.put<GenericResponseBase>(this._baseUrl + 'GlobalPlayer', request).toPromise();
  }

  public unreadyPlayer(request: IPlayerSession): Promise<GenericResponseBase> {
    return this._http.put<GenericResponseBase>(this._baseUrl + 'GlobalPlayer/Unready', ConvertToPlayerSessionRequest(request)).toPromise();
  }

  public again(request: IPlayerSession): Promise<GenericResponseBase> {
    var request = ConvertToPlayerSessionRequest(request);
    return this._http.post<GenericResponseBase>(this._baseUrl + 'GlobalPlayer/Again', request).toPromise();
  }

}
