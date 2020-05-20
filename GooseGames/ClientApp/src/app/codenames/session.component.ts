import { Component, Inject } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { GenericResponse, GenericResponseBase } from '../../models/genericresponse';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';


export enum CodenamesWordType {
  Blue = 1,
  Red = 2,
  Neutral = 3,
  Bomb = 4
}

export class CodenamesWord {
  word: string;
  id: string;
  wordType: CodenamesWordType;
  revealed: boolean;
}
export class CodenamesSession {
  sessionId: string;
  blueWordsRemaining: number;
  redWordsRemaining: number;
  gameOver: boolean;
  blueVictory: boolean;
  blueTurn: boolean;
  words: CodenamesWord[];

  wordRows: CodenamesWord[][];
}

export class PostWord {
  sessionId: string;
  wordId: string;
}
export class PatchWords {
  sessionId: string;
}
export class Pass {
  sessionId: string;
}


export abstract class CodenamesSessionComponentBase {

  _router: Router;

  GameIdentifier: string;
  Loading: boolean = true;
  ErrorMessage: string;
  _baseUrl: string;
  _http: HttpClient;
  Session: CodenamesSession;
  Spymaster: boolean = false;

  private _hubConnection: signalR.HubConnection;

  WordRevealedAnimation: CodenamesWord;

  constructor(router: Router, activatedRoute: ActivatedRoute, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._router = router;
    this._http = http;
    this._baseUrl = baseUrl;

    this.GameIdentifier = activatedRoute.snapshot.params.id.toLowerCase();

    this.loadSession()
      .then(() => {
        return this.setupConnection();
      });
  }

  loadSession(): Promise<any> {
    return this.load()
      .then(response => {
        if (response.success) {
          this.setSession(response);
        }
        else {
          this.ErrorMessage = response.errorCode;
        }

        this.Loading = false;
      })
      .catch(rer => this.handleGenericError(rer));
  }

    setSession(response: GenericResponse<CodenamesSession>) {
        this.Session = response.data;
    }

  load(): Promise<GenericResponse<CodenamesSession>> {
    return this._http.get<GenericResponse<CodenamesSession>>(this._baseUrl + `Codenames?gameIdentifier=${this.GameIdentifier}`).toPromise();
  }

  GuessWord(word: CodenamesWord) {

    if (this.Spymaster) {
      return;
    }

    var request = <PostWord>{
      sessionId: this.Session.sessionId,
      wordId: word.id
    }

    this._http.post<GenericResponseBase>(this._baseUrl + `Codenames/Reveal`, request).toPromise()
      .then(response => {
        if (response.errorCode) {
          this.ErrorMessage = response.errorCode;
        }
        else {
          this.RevealWord(word.id);
        }
      })
      .catch(err => this.handleGenericError(err));
  }

  PassToBlue() {
    if (this.Session.blueTurn) {
      return;
    }
    this.Pass();
  }

  PassToRed() {
    if (!this.Session.blueTurn) {
      return;
    }
    this.Pass();
  }

  Pass() {
    var request = <Pass>{
      sessionId: this.Session.sessionId
    }

    this._http.post<GenericResponseBase>(this._baseUrl + `Codenames/Pass`, request).toPromise()
      .then(response => {
        if (response.errorCode) {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(err => this.handleGenericError(err));
  }

  RevealWord(wordId: string) {
    var word = _.find(this.Session.words, w => w.id == wordId);
    if (word) {
      if (!word.revealed) {
        word.revealed = true;

        if (word.wordType == CodenamesWordType.Red) {
          this.Session.redWordsRemaining -= 1;
        }
        else if (word.wordType == CodenamesWordType.Blue)
        {
          this.Session.blueWordsRemaining -= 1;
        }
      }
    }

    this.WordRevealedAnimation = word;
  }

  RefreshWords() {

    var request = <PatchWords>{
      sessionId: this.Session.sessionId
    }

    this._http.post<GenericResponseBase>(this._baseUrl + `Codenames/Refresh`, request).toPromise()
      .then(response => {
        if (response.errorCode) {
          this.ErrorMessage = response.errorCode;
        }
        else {
          this.loadSession();
        }
      })
      .catch(err => this.handleGenericError(err));
  }

  handleGenericError(err: any) {
    console.error(err);
    this.ErrorMessage = "Unknown Error";
  }


  private setupConnection(): Promise<any> {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/codenameshub?sessionId=${this.Session.sessionId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {
      this.loadSession();
    });
    this._hubConnection.onclose(() => {
      this.handleGenericError("connection closed");
    });

    this._hubConnection.on("revealWord", (wordId: string) => {

      this.RevealWord(wordId);

    });
    this._hubConnection.on("wordsRefreshed", () => {

      this.loadSession();

    });

    this._hubConnection.on("isBlueTurn", (isBlueTurn) => {
      this.Session.blueTurn = isBlueTurn
    });

    this._hubConnection.on("isBlueVictory", (isBlueVictory) => {
      this.Session.gameOver = true;
      this.Session.blueVictory = isBlueVictory;
    });
    return this._hubConnection.start().catch(err => console.error(err));
  }
}


@Component({
  selector: 'app-codenames-session-component',
  templateUrl: './bigsession.component.html',
  styleUrls: ['./session.component.css']
})
export class BigCodenamesSessionComponent extends CodenamesSessionComponentBase{
  constructor(router: Router, activatedRoute: ActivatedRoute, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(router, activatedRoute, http, baseUrl);
  }
}



@Component({
  selector: 'app-codenames-session-component',
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css']
})
export class NormalCodenamesSessionComponent extends CodenamesSessionComponentBase {
  constructor(router: Router, activatedRoute: ActivatedRoute, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super(router, activatedRoute, http, baseUrl);
  }


  setSession(response: GenericResponse<CodenamesSession>) {
    this.Session = response.data;

    var words = this.Session.words;

    this.Session.wordRows = [];
    this.Session.wordRows.push(_.take(words, 5));
    this.Session.wordRows.push(_.take(_.drop(words, 5), 5));
    this.Session.wordRows.push(_.take(_.drop(words, 10), 5));
    this.Session.wordRows.push(_.take(_.drop(words, 15), 5));
    this.Session.wordRows.push(_.take(_.drop(words, 20), 5));
  }
}
