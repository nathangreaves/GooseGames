import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { PlayerSecretRoleResponse } from '../../models/werewords/playerroundinformation';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';

@Injectable({
  providedIn: 'root',
})

export class WerewordsPlayerRoundInformationService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public GetSecretRole(request: IPlayerSession): Promise<GenericResponse<PlayerSecretRoleResponse>> {

    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<PlayerSecretRoleResponse>>(this._baseUrl + `WerewordsPlayerRoundInformation/SecretRole?${parameters}`).toPromise();
  }

}
