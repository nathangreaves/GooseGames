import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { IPlayerSessionGame, ConvertToPlayerSessionGameRequest } from "../../models/session";
import { GenericResponse } from "../../models/genericresponse";
import { IAvalonPlayer } from "../../models/avalon/player";

@Injectable({
  providedIn: 'root',
})
export class AvalonPlayerService {

  constructor(private _http: HttpClient, @Inject('BASE_URL') private _baseUrl: string) {

  }

  public GetPlayers(playerSessionRequest: IPlayerSessionGame): Promise<GenericResponse<IAvalonPlayer[]>> {
    var request = ConvertToPlayerSessionGameRequest(playerSessionRequest);
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');

    return this._http.get<GenericResponse<IAvalonPlayer[]>>(`${this._baseUrl}AvalonPlayer?${parameters}`).toPromise();
  }
}
