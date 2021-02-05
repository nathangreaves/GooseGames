import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionRequest, ConvertToPlayerSessionGameRequest, IPlayerSession } from "../../models/session";
import { GenericResponse } from "../../models/genericresponse";
import { IAvalonRole, AvalonRoleEnum } from "../../models/avalon/roles";

@Injectable({
  providedIn: 'root',
})
export class AvalonRolesService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetAllRoles(playerSessionRequest: IPlayerSession): Promise<GenericResponse<IAvalonRole[]>> {
    var request = ConvertToPlayerSessionRequest(playerSessionRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IAvalonRole[]>>(`${this._baseUrl}AvalonRoles/GetAll?${parameters}`).toPromise();
  }

  public GetCurrentGameRoles(playerSessionGameRequest: IPlayerSessionGame): Promise<GenericResponse<IAvalonRole[]>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionGameRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IAvalonRole[]>>(`${this._baseUrl}AvalonRoles?${parameters}`).toPromise();
  }


  public GetWeight(request: IPlayerSession, selectedRoles: AvalonRoleEnum[]): Promise<GenericResponse<number>> {

    var request = ConvertToPlayerSessionRequest(request);
    (<any>request).roles = selectedRoles;
    return this._http.post<GenericResponse<number>>(this._baseUrl + 'AvalonRoles/GetWeight', request).toPromise();
  }

}
