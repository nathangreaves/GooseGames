import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse } from "../../models/genericresponse";
import { IGameEnd } from "../../models/letterjam/gameEnd";

@Injectable({
  providedIn: 'root',
})
export class LetterJamGameEndService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetGameEnd(playerSessionGameRequest: IPlayerSessionGame): Promise<GenericResponse<IGameEnd>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IGameEnd>>(`${this._baseUrl}LetterJamGameEnd?${parameters}`).toPromise();
  }
}
