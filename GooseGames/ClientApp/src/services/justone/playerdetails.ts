import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { UpdatePlayerDetailsRequest, UpdatePlayerDetailsResponse, GetPlayerDetailsRequest, GetPlayerDetailsResponse, DeletePlayerRequest } from '../../models/justone/player';

@Injectable({
  providedIn: 'root',
})

export class JustOnePlayerDetailsService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public UpdatePlayerDetails(request: UpdatePlayerDetailsRequest): Promise<GenericResponse<UpdatePlayerDetailsResponse>> {
    return this._http.patch<GenericResponse<UpdatePlayerDetailsResponse>>(this._baseUrl + 'JustOnePlayerDetails', request).toPromise();
  }

  public DeletePlayer(request: DeletePlayerRequest): Promise<GenericResponse<UpdatePlayerDetailsResponse>> {
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.delete<GenericResponse<UpdatePlayerDetailsResponse>>(this._baseUrl + 'JustOnePlayerDetails?'+ parameters).toPromise();
  }

  public GetPlayerDetails(request: GetPlayerDetailsRequest): Promise<GenericResponse<GetPlayerDetailsResponse>> {    
    var parameters = Object.keys(request).map(key => key + '=' + request[key]).join('&');
    return this._http.get<GenericResponse<GetPlayerDetailsResponse>>(this._baseUrl + 'JustOnePlayerDetails?' + parameters).toPromise();
  }
}
