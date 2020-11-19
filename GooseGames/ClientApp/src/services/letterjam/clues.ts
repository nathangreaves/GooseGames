import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse } from "../../models/genericresponse";
import { ITable, ITableNonPlayerCharacter, INonPlayerCharacter } from "../../models/letterjam/table";
import { ILetterJamRoundRequest } from "../../models/letterjam/content";
import { IProposedClue } from "../../models/letterjam/clues";

@Injectable({
  providedIn: 'root',
})
export class LetterJamCluesService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetClues(request: IPlayerSessionGame, roundId: string): Promise<GenericResponse<IProposedClue[]>> {
    var newRequest = <ILetterJamRoundRequest>{
      ...ConvertToPlayerSessionGameRequest(request),
      roundId: roundId
    };

    var parameters = Object.keys(newRequest).map(key => newRequest[key] ? key + '=' + newRequest[key] : '').join('&');

    return this._http.get<GenericResponse<IProposedClue[]>>(`${this._baseUrl}LetterJamClues/Proposed?${parameters}`).toPromise();
  }
}
