import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse } from "../../models/genericresponse";
import { ILetterCard } from "../../models/letterjam/letters";
import { ICardsRequest } from "../../models/letterjam/table";

@Injectable({
  providedIn: 'root',
})
export class LetterJamLetterCardService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetRelevantLetters(playerSessionGameRequest: IPlayerSessionGame): Promise<GenericResponse<ILetterCard[]>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<ILetterCard[]>>(`${this._baseUrl}LetterJamLetterCard/RelevantLetters?${parameters}`).toPromise();
  }

  public GetLetters(playerSessionGameRequest: IPlayerSessionGame, letterCardsRequest: ICardsRequest): Promise<GenericResponse<ILetterCard[]>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    (<any>request).cardIds = letterCardsRequest.cardIds;

    return this._http.post<GenericResponse<ILetterCard[]>>(`${this._baseUrl}LetterJamLetterCard/GetLetters`, request).toPromise();
  }
}
