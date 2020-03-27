import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOneSessionService } from '../../services/justone/session'
import { GenericResponse } from '../../models/genericresponse'
import { SessionLandingResponse } from '../../models/justone/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-just-one-landing-component',
  templateUrl: './landing.component.html'
})

export class JustOneLandingComponent {

  private _sessionService: JustOneSessionService;
  private _router: Router;

  public ErrorMessage: string;

  constructor(sessionService: JustOneSessionService, router: Router) {
    this._sessionService = sessionService;
    this._router = router;
  }


  public StartNewGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.ErrorMessage = "No password given for new game";
      return;
    }

    this._sessionService.CreateGame({ password: password })
      .then(data => this.handleResponse(data))
      .catch(data => this.genericError());

  }

  public JoinGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.ErrorMessage = "No password given for joining game";
      return;
    }

    this._sessionService.JoinGame({ password: password })
      .then(data => this.handleResponse(data))
      .catch(data => this.genericError());
  }

  private handleResponse(data: GenericResponse<SessionLandingResponse>) {
    if (data.success === true) {
      this.clearMessage();

      this.ErrorMessage = `Session Id: ${data.data.sessionId} :: Player Id: ${data.data.playerId}`;

      this._router.navigate(['/justone/newplayer', { sessionId: data.data.sessionId, playerId: data.data.playerId }]);

      //Redirect to add player
      //this.Message = "Game added " + data.gameId;
      //this.Games.push({ Id: data.gameId, Password: password, NumberOfPlayers: 1 });
    }
    else {
      //Stay on page and display error
      this.ErrorMessage = data.errorCode;
    }
  }

  genericError() {
    this.ErrorMessage = "An unexpected error occurred";
  }

  clearMessage() {
    this.ErrorMessage = null;
  }
}
