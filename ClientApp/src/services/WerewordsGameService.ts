import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateWerewordsGameResponse } from '../models/werewords/CreateWerewordsGameResponse';
import { JoinWerewordsGameResponse } from '../models/werewords/JoinWerewordsGameResponse';

@Injectable({
  providedIn: 'root',
})

export class WerewordsGameService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public CreateGame(password: string): Promise<CreateWerewordsGameResponse>
  {
    return this._http.post<CreateWerewordsGameResponse>(this._baseUrl + 'werewordsgame', { password: password }).toPromise();
  }

  public JoinGame(password: string): Promise<JoinWerewordsGameResponse> {
    return this._http.patch<JoinWerewordsGameResponse>(this._baseUrl + 'werewordsgame', { password: password }).toPromise();
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
