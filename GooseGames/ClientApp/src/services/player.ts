import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../models/session';
import { IGooseGamesPlayer } from '../models/player';

@Injectable({
  providedIn: 'root',
})
export class GlobalPlayerService {

  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public getPlayers(request: IPlayerSession): Promise<GenericResponse<IGooseGamesPlayer[]>> {

    var request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<IGooseGamesPlayer[]>>(this._baseUrl + 'GlobalPlayer?' + parameters).toPromise();
  }
}
