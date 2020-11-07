import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericResponse } from '../../models/genericresponse';
import { SessionLandingRequest, SessionLandingResponse, IPlayerSession } from '../../models/session';
import { JustOneWordList } from '../../models/justone/wordlistenum';

@Injectable({
  providedIn: 'root',
})

export class JustOneSessionService {
  private _baseUrl: string;
  private _http: HttpClient;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this._baseUrl = baseUrl;
    this._http = http;
  }

  public StartSession(request: IPlayerSession, wordLists: JustOneWordList[]): Promise<GenericResponse<boolean>> {

    var newRequest = <any>(<IPlayerSession>{
      SessionId: request.SessionId,
      PlayerId: request.PlayerId
    })
    newRequest.IncludedWordLists = wordLists;

    return this._http.post<GenericResponse<boolean>>(this._baseUrl + 'JustOneSession/startsession', newRequest).toPromise();
  }
}
