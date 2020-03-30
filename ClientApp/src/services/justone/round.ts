import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { PlayerAction, PassivePlayerRoundInformationResponse } from '../../models/justone/playeractions';
import { IPlayerSession, ConvertToPlayerSessionRequest, PlayerSessionRequest } from '../../models/justone/session';
import { RoundOutcomeResponse } from '../../models/justone/round';

@Injectable({
  providedIn: 'root',
})

export class JustOneRoundService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public GetPlayerResponseInformation(request: IPlayerSession): Promise<GenericResponse<PlayerAction[]>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerAction[]>>(this._baseUrl + `JustOnePlayerAction/ResponseInfo?${parameters}`).toPromise();
  }

  public GetPlayerResponseVoteInformation(request: IPlayerSession): Promise<GenericResponse<PlayerAction[]>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerAction[]>>(this._baseUrl + `JustOnePlayerAction/ResponseVoteInfo?${parameters}`).toPromise();
  }

  public GetPlayerOutcomeVoteInformation(request: IPlayerSession): Promise<GenericResponse<PlayerAction[]>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerAction[]>>(this._baseUrl + `JustOnePlayerAction/ResponseVoteInfo?${parameters}`).toPromise();
  }

  public GetRoundForPassivePlayer(request: IPlayerSession): Promise<GenericResponse<PassivePlayerRoundInformationResponse>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PassivePlayerRoundInformationResponse>>(this._baseUrl + `JustOneRound/PassivePlayerInfo?${parameters}`).toPromise();
  }

  public GetRoundOutcome(request: PlayerSessionRequest): Promise<GenericResponse<RoundOutcomeResponse>> {
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<RoundOutcomeResponse>>(this._baseUrl + `JustOneRound/Outcome?${parameters}`).toPromise();
  }
}
