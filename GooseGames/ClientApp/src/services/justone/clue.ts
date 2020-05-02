import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';
import { SubmitClueRequest, SubmitClueVotesRequest, PlayerClue, PlayerCluesResponse, SubmitActivePlayerResponseRequest } from '../../models/justone/clue';

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

  public GetClues(request: IPlayerSession): Promise<GenericResponse<PlayerCluesResponse>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerCluesResponse>>(this._baseUrl + `JustOnePlayerResponse?${parameters}`).toPromise();
  }

  public SubmitClue(request: SubmitClueRequest): Promise<GenericResponseBase> {
    return this._http.post<GenericResponseBase>(this._baseUrl + 'JustOnePlayerResponse/SubmitResponse', request).toPromise();
  }

  public SubmitClueVote(request: SubmitClueVotesRequest): Promise<GenericResponseBase> {
    return this._http.post<GenericResponseBase>(this._baseUrl + 'JustOnePlayerResponse/SubmitResponseVote', request).toPromise();
  }

  public SubmitActivePlayerResponse(request: SubmitActivePlayerResponseRequest): Promise<GenericResponseBase> {
    return this._http.post<GenericResponseBase>(this._baseUrl + 'JustOnePlayerResponse/SubmitActivePlayerResponse', request).toPromise();
  }

  public SubmitActivePlayerResponseVote(request: SubmitClueVotesRequest): Promise<GenericResponseBase> {
    return this._http.post<GenericResponseBase>(this._baseUrl + 'JustOnePlayerResponse/SubmitActivePlayerResponseVote', request).toPromise();
  }

  public GetActivePlayerResponse(request: IPlayerSession): Promise<GenericResponse<PlayerCluesResponse>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerCluesResponse>>(this._baseUrl + `JustOnePlayerResponse/ActivePlayerResponse?${parameters}`).toPromise();
  }
}
