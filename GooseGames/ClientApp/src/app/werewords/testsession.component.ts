import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router } from '@angular/router';
import { SessionLandingResponse } from '../../models/session';
import { WerewordsSessionService } from '../../services/werewords/session';


@Component({
  selector: 'app-werewords-test-session-component',
  templateUrl: './testsession.component.html'
})


export class WerewordsTestSessionComponent {

  _router: Router;

  ErrorMessage: string;
  GameId: string;
  PostedGameId: string;
  Responses: SessionLandingResponse[];
  _sessionService: WerewordsSessionService;

  getRandomInt(max: number = 2147483648) {
  return Math.floor(Math.random() * Math.floor(max));
}

  constructor(sessionService: WerewordsSessionService, router: Router) {
    this._router = router;
    this._sessionService = sessionService;

    this.GameId = this.getRandomInt().toString();
  }

  Go() {
    this.Responses = null;
    this.ErrorMessage = null;
    const gameId = this.GameId;
    this._sessionService.CreateTestSession(gameId).then(response => {
      if (response.success) {
        this.PostedGameId = gameId;
        this.Responses = response.data;
      }
      else {
        this.ErrorMessage = response.errorCode;
      }
    });

  }

}
