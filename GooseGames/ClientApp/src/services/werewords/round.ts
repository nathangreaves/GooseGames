import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { PlayerSecretRoleResponse } from '../../models/werewords/playerroundinformation';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';

@Injectable({
  providedIn: 'root',
})

export class WerewordsRoundService {
  
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public GetWords(request: IPlayerSession): Promise<GenericResponse<string[]>> {

    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<string[]>>(this._baseUrl + `WerewordsRound/Words?${parameters}`).toPromise();
  }

  SelectWord(playerSession: IPlayerSession, word: string) : Promise<GenericResponseBase> {
    var request = ConvertToPlayerSessionRequest(playerSession);
    (<any>request).word = word;

    return this._http.post<GenericResponseBase>(this._baseUrl + 'WerewordsRound/Word', request).toPromise();
  }
}
