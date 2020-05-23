import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { PlayerSecretRoleResponse, SecretWordResponse, PlayerRoundInformation, PlayerResponseType } from '../../models/werewords/playerroundinformation';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';
import { DayResponse, RoundOutcomeResponse } from '../../models/werewords/round';

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

    return this._http.post<GenericResponseBase>(this._baseUrl + 'WerewordsRound/SecretWord', request).toPromise();
  }

  GetSecretWord(playerSession: IPlayerSession): Promise<GenericResponse<SecretWordResponse>> {
    var request = ConvertToPlayerSessionRequest(playerSession);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');    

    return this._http.get<GenericResponse<SecretWordResponse>>(this._baseUrl + `WerewordsRound/SecretWord?${parameters}`).toPromise();
  }

  GetDay(playerSession: IPlayerSession): Promise<GenericResponse<DayResponse>> {
    var request = ConvertToPlayerSessionRequest(playerSession);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<DayResponse>>(this._baseUrl + `WerewordsRound/Day?${parameters}`).toPromise();
  }

  PlayerResponse(playerSession: IPlayerSession, player: PlayerRoundInformation, responseType: PlayerResponseType): Promise<GenericResponseBase>  {
    var request = ConvertToPlayerSessionRequest(playerSession);
    (<any>request).respondingPlayerId = player.id;
    (<any>request).responseType = responseType;

    return this._http.post<GenericResponseBase>(this._baseUrl + 'WerewordsRound/PlayerResponse', request).toPromise();
  }

  Start(playerSession: IPlayerSession) {
    var request = ConvertToPlayerSessionRequest(playerSession);
    return this._http.post<GenericResponseBase>(this._baseUrl + 'WerewordsRound/Start', request).toPromise();
  }

  VoteAsSeer(playerSession: IPlayerSession, nominatedPlayerId: string) {
    var request = ConvertToPlayerSessionRequest(playerSession);
    (<any>request).nominatedPlayerId = nominatedPlayerId;

    return this._http.post<GenericResponseBase>(this._baseUrl + 'WerewordsRound/VoteAsSeer', request).toPromise();
  }
  VoteAsWerewolf(playerSession: IPlayerSession, nominatedPlayerId: string) {
    var request = ConvertToPlayerSessionRequest(playerSession);
    (<any>request).nominatedPlayerId = nominatedPlayerId;

    return this._http.post<GenericResponseBase>(this._baseUrl + 'WerewordsRound/VoteAsWerewolf', request).toPromise();
  }

  GetOutcome(playerSession: IPlayerSession): Promise<GenericResponse<RoundOutcomeResponse>> {

    var request = ConvertToPlayerSessionRequest(playerSession);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<RoundOutcomeResponse>>(this._baseUrl + `WerewordsRound/Outcome?${parameters}`).toPromise();
  }
}
