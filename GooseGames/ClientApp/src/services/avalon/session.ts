import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';
import { IAvalonComponentBase, AvalonPlayerStatus } from '../../models/avalon/content';
import { PlayerStatusValidationResponse, PlayerStatusGameValidationResponse } from '../../models/player';
import { AvalonRoleEnum } from '../../models/avalon/roles';

@Injectable({
  providedIn: 'root',
})
export class AvalonSessionService {

  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public StartSession(request: IPlayerSession, selectedRoles: AvalonRoleEnum[]): Promise<GenericResponseBase> {

    var request = ConvertToPlayerSessionRequest(request);
    (<any>request).roles = selectedRoles;
    return this._http.post<GenericResponseBase>(this._baseUrl + 'AvalonLobby/StartSession', request).toPromise();
  }

  public Again(request: IPlayerSession): Promise<GenericResponseBase> {
    var request = ConvertToPlayerSessionRequest(request);
    return this._http.post<GenericResponseBase>(this._baseUrl + 'GlobalPlayer/Again', request).toPromise();
  }


  public Validate(component: IAvalonComponentBase, onReroute: Function): Promise<GenericResponse<PlayerStatusGameValidationResponse>> {

    var request = <IPlayerSession>{
      SessionId: component.SessionId,
      PlayerId: component.PlayerId
    }
    if (component.GameId) {
      (<any>request).GameId = component.GameId;
    }

    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<PlayerStatusGameValidationResponse>>(this._baseUrl + `AvalonLobby/ValidateStatus?${parameters}`)
      .toPromise()
      .then((response) => {
        if (response.success) {
          if (response.data.gameId) {
            component.SetGameId(response.data.gameId);
          }
          if (!response.data.statusCorrect) {
            onReroute();
            component.router(AvalonPlayerStatus[response.data.requiredStatus], true);
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
        return new GenericResponse<PlayerStatusGameValidationResponse>();
      });
  }

}
