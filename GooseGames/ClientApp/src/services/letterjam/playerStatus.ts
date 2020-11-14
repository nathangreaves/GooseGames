import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest, IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from '../../models/session'
import { LetterJamPlayerStatus, ILetterJamComponentBase } from '../../models/letterjam/content';
import { PlayerStatusValidationResponse, PlayerAction, IGooseGamesPlayerAction } from '../../models/player';
import { LetterJamPlayerStatusValidationResponse } from '../../models/letterjam/playerStatus';

@Injectable({
  providedIn: 'root',
})

export class LetterJamPlayerStatusService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public Validate(component: ILetterJamComponentBase, playerStatus: LetterJamPlayerStatus, onReroute: Function): Promise<GenericResponse<PlayerStatusValidationResponse>> {

    var request = <IPlayerSession>{
      SessionId: component.SessionId,
      PlayerId: component.PlayerId
    }
    if (component.GameId) {
      (<any>request).GameId = component.GameId;
    }

    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<LetterJamPlayerStatusValidationResponse>>(this._baseUrl + `LetterJamPlayerStatus/${LetterJamPlayerStatus[playerStatus]}?${parameters}`)
      .toPromise()
      .then((response) => {
        if (response.success) {
          if (response.data.gameId) {
            component.SetGameId(response.data.gameId);
          }
          if (!response.data.statusCorrect) {
            onReroute();
            component.router(LetterJamPlayerStatus[response.data.requiredStatus], true);
            response.success = false;
            component.SetErrorMessage("Rerouting to correct page...");
          }
        }
        else {

          if (response.errorCode == "511c0fb3-7d49-4fdf-a1a7-b1281b5ada4b" || response.errorCode == "a530d7fa-f842-492b-a0fc-6473af1c907a") {
            //Has been previously booted from session, attempt rejoin.


          }
          else {
            component.SetErrorMessage(response.errorCode);
          }
        }
        return response;
      })
      .catch((err) => {
        component.HandleGenericError(err);
        return new GenericResponse<PlayerStatusValidationResponse>();
      });
  }


  PlayersWaitingForFirstRound(request: IPlayerSessionGame): Promise<GenericResponse<IGooseGamesPlayerAction[]>> {

    var request = ConvertToPlayerSessionGameRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IGooseGamesPlayerAction[]>>(this._baseUrl + `LetterJamStartWord/PlayerActions?${parameters}`)
      .toPromise();
  }
}
