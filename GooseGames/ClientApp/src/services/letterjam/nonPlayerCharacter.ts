import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse } from "../../models/genericresponse";
import { ITable, ITableNonPlayerCharacter, INonPlayerCharacter } from "../../models/letterjam/table";

@Injectable({
  providedIn: 'root',
})
export class LetterJamNonPlayerCharacterService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetNonPlayerCharacters(request: IPlayerSessionGame): Promise<GenericResponse<INonPlayerCharacter[]>> {
    var request = ConvertToPlayerSessionGameRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<INonPlayerCharacter[]>>(`${this._baseUrl}LetterJamNonPlayerCharacter?${parameters}`).toPromise();
  }
}
