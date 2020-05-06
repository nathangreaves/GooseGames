import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { IPlayerSession, ConvertToPlayerSessionRequest } from '../../models/session';
import { FujiPlayerHand } from '../../models/fujiflush/hand';
import { FujiCard, FujiHandCard } from '../../models/fujiflush/card';

@Injectable({
  providedIn: 'root',
})

export class FujiCardService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public PlayCard(request: IPlayerSession, cardId: string): Promise<GenericResponseBase> {
    var newRequest = <any>ConvertToPlayerSessionRequest(request);
    newRequest.CardId = cardId;
    return this._http.post<GenericResponseBase>(this._baseUrl + 'FujiCard', newRequest).toPromise();
  }
  public GetCard(newCardId: string): Promise<GenericResponse<FujiHandCard>> {
    return this._http.get<GenericResponse<FujiHandCard>>(this._baseUrl + `FujiCard?CardId=${newCardId}`).toPromise();
  }
}
