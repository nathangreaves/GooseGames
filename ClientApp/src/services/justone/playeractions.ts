import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { PlayerAction } from '../../models/justone/playeractions';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/justone/session';

@Injectable({
  providedIn: 'root',
})

export class JustOnePlayerActionsService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }
    //var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

  public GetPlayerResponseInformation(request: IPlayerSession): Promise<GenericResponse<PlayerAction[]>> {
    return this._http.patch<GenericResponse<PlayerAction[]>>(this._baseUrl + 'JustOnePlayerResponse/Info', ConvertToPlayerSessionRequest(request)).toPromise();
  }
}
