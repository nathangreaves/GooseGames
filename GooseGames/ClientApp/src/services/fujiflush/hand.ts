import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';
import { FujiPlayerHand } from '../../models/fujiflush/hand';

@Injectable({
  providedIn: 'root',
})

export class FujiHandService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public GetHand(request: IPlayerSession): Promise<GenericResponse<FujiPlayerHand>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<FujiPlayerHand>>(this._baseUrl + `FujiHand?${parameters}`).toPromise();
  }
}
