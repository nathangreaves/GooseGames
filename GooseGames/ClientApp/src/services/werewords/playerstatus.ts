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
          component.SetErrorMessage(response.errorCode);
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

  //public SetRoundWaiting(request: PlayerSessionRoundRequest): Promise<GenericResponse<boolean>> {
  //  return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.RoundWaiting]}`, request).toPromise();
  //}

  //public SetPassivePlayerClue(request: IPlayerSession): Promise<GenericResponse<boolean>> {
  //  return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.PassivePlayerClue]}`, ConvertToPlayerSessionRequest(request)).toPromise();
  //}

  //public SetPassivePlayerClueVote(request: IPlayerSession): Promise<GenericResponse<boolean>> {
  //  return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.PassivePlayerClueVote]}`, ConvertToPlayerSessionRequest(request)).toPromise();
  //}

  //public SetPassivePlayerOutcomeVote(request: IPlayerSession): Promise<GenericResponse<boolean>> {
  //  return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.PassivePlayerOutcomeVote]}`, ConvertToPlayerSessionRequest(request)).toPromise();
  //}

  //public Set(playerId: string, playerStatus: PlayerStatus): Promise<GenericResponse<boolean>> {
  //  return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[playerStatus]}`, { PlayerId: playerId }).toPromise();
  //}
}
