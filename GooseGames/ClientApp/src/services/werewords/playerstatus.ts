import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session'
import { WerewordsPlayerStatus, IWerewordsComponentBase } from '../../models/werewords/content';
import { PlayerStatusValidationResponse, PlayerAction } from '../../models/player';

@Injectable({
  providedIn: 'root',
})

export class WerewordsPlayerStatusService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public Validate(component: IWerewordsComponentBase, playerStatus: WerewordsPlayerStatus, onReroute: Function): Promise<GenericResponse<PlayerStatusValidationResponse>> {

    var request = <IPlayerSession>{
      SessionId: component.SessionId,
      PlayerId: component.PlayerId
    }

    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerStatusValidationResponse>>(this._baseUrl + `WerewordsPlayerStatus/${WerewordsPlayerStatus[playerStatus]}?${parameters}`)
      .toPromise()
      .then((response) => {
        if (response.success) {
          if (!response.data.statusCorrect) {
            onReroute();
            component.router(WerewordsPlayerStatus[response.data.requiredStatus], true);
            response.success = false;
            component.SetErrorMessage("Rerouting to correct page...");
          }
        }
        else {

          if (response.errorCode == "511c0fb3-7d49-4fdf-a1a7-b1281b5ada4b" || response.errorCode == "a530d7fa-f842-492b-a0fc-6473af1c907a") {
            //Has been previously booted from session, attempt rejoin.


          }
          else {
            component.SetErrorMessage(response.errorCode); }
        }
        return response;
      })
      .catch((err) => {
        component.HandleGenericError(err);
        return new GenericResponse<PlayerStatusValidationResponse>();
      });
  }


  TransitionToNextNightStatus(playerSession: IPlayerSession): Promise<GenericResponse<WerewordsPlayerStatus>> {
    var request = <IPlayerSession>{
      SessionId: playerSession.SessionId,
      PlayerId: playerSession.PlayerId
    };

    return this._http.post<GenericResponse<string>>(this._baseUrl + `WerewordsPlayerStatus/TransitionNight`, request).toPromise().then(response =>
    {
      return <GenericResponse<WerewordsPlayerStatus>>{
        success: response.success,
        errorCode: response.errorCode,
        data: response.success ? WerewordsPlayerStatus[response.data] : null
      };
    });
  }


  public GetAwakePlayers(request: IPlayerSession): Promise<GenericResponse<PlayerAction[]>> {
    request = ConvertToPlayerSessionRequest(request);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerAction[]>>(this._baseUrl + `WerewordsPlayerAction/PlayersAwake?${parameters}`).toPromise();
  }
}
