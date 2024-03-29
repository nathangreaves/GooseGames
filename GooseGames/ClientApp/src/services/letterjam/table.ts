import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse } from "../../models/genericresponse";
import { ITable, ITableNonPlayerCharacter, INonPlayerCharacter, IRound } from "../../models/letterjam/table";

@Injectable({
  providedIn: 'root',
})
export class LetterJamTableService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetTable(playerSessionGameRequest: IPlayerSessionGame): Promise<GenericResponse<ITable>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<ITable>>(`${this._baseUrl}LetterJamTable?${parameters}`).toPromise();
  }

  public GetCurrentRound(playerSessionGameRequest: IPlayerSessionGame): Promise<GenericResponse<IRound>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IRound>>(`${this._baseUrl}LetterJamTable/CurrentRound?${parameters}`).toPromise();
  }
}
