import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse, GenericResponseBase } from "../../models/genericresponse";
import { IMyJam, ILetterGuess, IFinalWordLetter, IFinalWordPublicLetter } from "../../models/letterjam/myJam";

@Injectable({
  providedIn: 'root',
})
export class LetterJamMyJamService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public LoadMyJam(playerSessionGameRequest: IPlayerSessionGame): Promise<GenericResponse<IMyJam>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IMyJam>>(`${this._baseUrl}LetterJamMyJam?${parameters}`).toPromise();
  }

  public PostLetterGuesses(playerSessionGameRequest: IPlayerSessionGame, letterGuesses: ILetterGuess[], moveOnToNextLetter: boolean): Promise<GenericResponseBase> {
    var request = {
      ...ConvertToPlayerSessionGameRequest(playerSessionGameRequest),
      letterGuesses: letterGuesses,
      moveOnToNextLetter: moveOnToNextLetter
    };
    return this._http.post<GenericResponseBase>(`${this._baseUrl}LetterJamMyJam/LetterGuesses`, request).toPromise();
  }

  public PostFinalWord(playerSessionGameRequest: IPlayerSessionGame, letterGuesses: ILetterGuess[], finalWordLetters: IFinalWordLetter[]): Promise<GenericResponseBase> {
    var request = {
      ...ConvertToPlayerSessionGameRequest(playerSessionGameRequest),
      letterGuesses: letterGuesses,
      finalWordLetters: finalWordLetters.map(l => {
        return {
          letterId: l.cardId,
          isWildCard: l.isWildCard
        }
      })
    };
    return this._http.post<GenericResponseBase>(`${this._baseUrl}LetterJamFinalWord`, request).toPromise();
  }

  public GetFinalWordPublicLetters(playerSessionGameRequest: IPlayerSessionGame) {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<IFinalWordPublicLetter[]>>(`${this._baseUrl}LetterJamFinalWord/PublicLetters?${parameters}`).toPromise();
  }
  public GetFinalWord(playerSessionGameRequest: IPlayerSessionGame) {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<IFinalWordLetter[]>>(`${this._baseUrl}LetterJamFinalWord?${parameters}`).toPromise();
  }
}
