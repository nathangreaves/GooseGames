import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/justone/session';
import { PlayerStatusValidationResponse } from '../../models/justone/playerstatus';
import { SubmitClueRequest, SubmitClueVotesRequest, PlayerClue } from '../../models/justone/clue';

@Injectable({
  providedIn: 'root',
})

export class JustOneClueService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public GetClues(request: IPlayerSession): Promise<GenericResponse<PlayerClue[]>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerClue[]>>(this._baseUrl + `JustOnePlayerResponse?${parameters}`).toPromise();
  }

  public SubmitClue(request: SubmitClueRequest): Promise<GenericResponseBase> {
    return this._http.post<GenericResponseBase>(this._baseUrl + 'JustOnePlayerResponse/SubmitClue', request).toPromise();
  }

  public SubmitClueVote(request: SubmitClueVotesRequest): Promise<GenericResponseBase> {
    return this._http.post<GenericResponseBase>(this._baseUrl + 'JustOnePlayerResponse/SubmitClueVote', request).toPromise();
  }
}
