import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from '../../models/session'
import { ILetterJamStartWordConfiguration, ILetterJamRandomWord } from '../../models/letterjam/startWord';

@Injectable({
  providedIn: 'root',
})

export class LetterJamStartWordService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public GetConfiguration(playerSessionGameRequest: IPlayerSessionGame): Promise<GenericResponse<ILetterJamStartWordConfiguration>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<ILetterJamStartWordConfiguration>>(`${this._baseUrl}LetterJamStartWord/Configuration?${parameters}`).toPromise();
  }

  public GetRandomWord(playerSessionGameRequest: IPlayerSessionGame, numberOfLetters: number): Promise<GenericResponse<ILetterJamRandomWord>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    (<any>request).NumberOfLetters = numberOfLetters;
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<ILetterJamRandomWord>>(`${this._baseUrl}LetterJamStartWord/RandomWord?${parameters}`).toPromise();
  }

  public PostStartWord(playerSessionGameRequest: IPlayerSessionGame, startWord: string, forPlayerId: string): Promise<GenericResponseBase> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    (<any>request).StartWord = startWord;
    (<any>request).ForPlayerId = forPlayerId;

    return this._http.post<GenericResponseBase>(`${this._baseUrl}LetterJamStartWord`, request).toPromise();
  }
}
