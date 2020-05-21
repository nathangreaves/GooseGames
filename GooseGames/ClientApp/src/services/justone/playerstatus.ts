import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { IPlayerSession, IPlayerSessionComponent, PlayerSessionRequest, ConvertToPlayerSessionRequest } from '../../models/session'
import { PlayerStatus, PlayerStatusRoutesMap } from '../../models/justone/playerstatus';
import { Router } from '@angular/router';
import { PlayerSessionRoundRequest } from '../../models/justone/round';
import { PlayerStatusValidationResponse } from '../../models/player';

@Injectable({
  providedIn: 'root',
})

export class JustOnePlayerStatusService {
  private _baseUrl: string;
  private _http: HttpClient;
  private _router: Router;

  constructor(http: HttpClient, router: Router, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
    this._router = router;
  }

  public Reroute(reroute: PlayerStatus, playerSession: IPlayerSession) {

    var enumName = PlayerStatus[reroute];
    var route = PlayerStatusRoutesMap[enumName];

    this._router.navigate([route, playerSession]);
  }

  public Validate(component: IPlayerSessionComponent, playerStatus: PlayerStatus, onReroute: Function): Promise<GenericResponse<PlayerStatusValidationResponse>> {

    var request = <IPlayerSession>{
      SessionId: component.SessionId,
      PlayerId: component.PlayerId
    }

    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerStatusValidationResponse>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[playerStatus]}?${parameters}`)
      .toPromise()
      .then((response) => {
        if (response.success) {
          if (!response.data.statusCorrect) {
            onReroute();
            this.Reroute(PlayerStatus[response.data.requiredStatus], request);
            response.success = false;
            component.ErrorMessage = "Rerouting to correct page...";
          }
        }
        else {
          component.ErrorMessage = response.errorCode;
        }
        return response;
      })
      .catch(() => {
        component.HandleGenericError();
        return new GenericResponse<PlayerStatusValidationResponse>();
      });
  }

  public SetRoundWaiting(request: PlayerSessionRoundRequest): Promise<GenericResponse<boolean>> {
    return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.RoundWaiting]}`, request).toPromise();
  }

  public SetPassivePlayerClue(request: IPlayerSession): Promise<GenericResponse<boolean>> {
    return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.PassivePlayerClue]}`, ConvertToPlayerSessionRequest(request)).toPromise();
  }

  public SetPassivePlayerClueVote(request: IPlayerSession): Promise<GenericResponse<boolean>> {
    return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.PassivePlayerClueVote]}`, ConvertToPlayerSessionRequest(request)).toPromise();
  }

  public SetPassivePlayerOutcomeVote(request: IPlayerSession): Promise<GenericResponse<boolean>> {
    return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[PlayerStatus.PassivePlayerOutcomeVote]}`, ConvertToPlayerSessionRequest(request)).toPromise();
  }

  public Set(playerId: string, playerStatus: PlayerStatus): Promise<GenericResponse<boolean>> {
    return this._http.post<GenericResponse<boolean>>(this._baseUrl + `JustOnePlayerStatus/${PlayerStatus[playerStatus]}`, { PlayerId: playerId }).toPromise();
  }
}
