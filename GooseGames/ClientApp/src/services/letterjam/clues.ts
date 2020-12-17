import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse, GenericResponseBase } from "../../models/genericresponse";
import { ILetterJamRoundRequest } from "../../models/letterjam/content";
import { IProposedClue, IClueLetter, IProposedClues } from "../../models/letterjam/clues";
import { LetterCard } from "../../models/letterjam/letters";
import _ from "lodash";

const WildCardId: string = "dd4750cc-07cf-497e-867d-6f434938677e";

@Injectable({
  providedIn: 'root',
})
export class LetterJamCluesService {
  

  WildCardId: string = WildCardId;

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetClues(request: IPlayerSessionGame, roundId: string): Promise<GenericResponse<IProposedClues>> {
    var newRequest = <ILetterJamRoundRequest>{
      ...ConvertToPlayerSessionGameRequest(request),
      roundId: roundId
    };

    var parameters = Object.keys(newRequest).map(key => newRequest[key] ? key + '=' + newRequest[key] : '').join('&');

    return this._http.get<GenericResponse<IProposedClues>>(`${this._baseUrl}LetterJamClues/Proposed?${parameters}`).toPromise();
  }

  public SubmitClue(request: IPlayerSessionGame, roundId: string, letterCards: LetterCard[]): Promise<GenericResponseBase> {

    var letters = [];
    _.each(letterCards, c => letters.push({
      letterId: c.cardId !== WildCardId ? c.cardId : null,
      isWildCard: c.cardId == WildCardId
    }));

    var postRequest = {
      ...ConvertToPlayerSessionGameRequest(request),
      roundId: roundId,
      clueLetters: letters
    };

    return this._http.post<GenericResponseBase>(`${this._baseUrl}LetterJamClues`, postRequest).toPromise();
  }

  public Vote(request: IPlayerSessionGame, roundId: string, clueId: string): Promise<GenericResponseBase> {

    var postRequest = {
      ...ConvertToPlayerSessionGameRequest(request),
      roundId: roundId,
      clueId: clueId
    }

    return this._http.post<GenericResponseBase>(`${this._baseUrl}LetterJamClueVote`, postRequest).toPromise();
  }

  public GiveClue(request: IPlayerSessionGame, roundId: string, clueId: string): Promise<GenericResponseBase> {

    var postRequest = {
      ...ConvertToPlayerSessionGameRequest(request),
      roundId: roundId,
      clueId: clueId
    }

    return this._http.post<GenericResponseBase>(`${this._baseUrl}LetterJamClues/GiveClue`, postRequest).toPromise();
  }

  public GetLettersForClue(request: IPlayerSessionGame, clueId: string): Promise<GenericResponse<IClueLetter[]>> {
    var request = ConvertToPlayerSessionGameRequest(request);
    (<any>request).clueId = clueId;
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IClueLetter[]>>(`${this._baseUrl}LetterJamClues/GetLettersForClue?${parameters}`).toPromise();
  }

  public DeleteClue(request: IPlayerSessionGame, clueId: string): Promise<GenericResponseBase> {
    var request = ConvertToPlayerSessionGameRequest(request);
    (<any>request).clueId = clueId; 
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.delete<GenericResponseBase>(`${this._baseUrl}LetterJamClues?${parameters}`).toPromise();
  }
}
