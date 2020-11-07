import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { SessionLandingRequest, SessionLandingResponse, IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';
import { UpdatePlayerDetailsRequest, GetPlayerDetailsResponse, DeletePlayerRequest } from '../../models/player';

@Injectable({
  providedIn: 'root',
})
export class WerewordsSessionService {
  
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public StartSession(request: IPlayerSession): Promise<GenericResponse<boolean>> {

    var request = ConvertToPlayerSessionRequest(request);
    return this._http.post<GenericResponse<boolean>>(this._baseUrl + 'WerewordsLobby/StartSession', request).toPromise();
  }

  public Again(request: IPlayerSession): Promise<GenericResponseBase> {
    var request = ConvertToPlayerSessionRequest(request);
    return this._http.post<GenericResponseBase>(this._baseUrl + 'GlobalPlayer/Again', request).toPromise();
  }

}
