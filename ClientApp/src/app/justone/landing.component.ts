import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOneSessionService } from '../../services/justone/session'
import { GenericResponse } from '../../models/genericresponse'
import { SessionLandingResponse } from '../../models/justone/session'

@Component({
  selector: 'app-just-one-landing-component',
  templateUrl: './landing.component.html'
})

export class JustOneLandingComponent {

  private _sessionService: JustOneSessionService;

  public Message: string;

  constructor(sessionService: JustOneSessionService) {
    this._sessionService = sessionService;
  }


  public StartNewGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.Message = "No password given for new game";
      return;
    }

    this._sessionService.CreateGame({ password: password })
      .then(this.handleResponse)
      .catch(this.genericError);

  }

  public JoinGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.Message = "No password given for joining game";
      return;
    }

    this._sessionService.JoinGame({ password: password })
      .then(this.handleResponse)
      .catch(this.genericError);
  }

  private handleResponse(data: GenericResponse<SessionLandingResponse>) {
    if (data.success === true) {
      this.clearMessage();

      //Redirect to add player
      //this.Message = "Game added " + data.gameId;
      //this.Games.push({ Id: data.gameId, Password: password, NumberOfPlayers: 1 });
    }
    else {
      //Stay on page and display error
      this.Message = data.errorCode;
    }
  }

  genericError() {
    this.Message = "An unexpected error occurred";
  }

  clearMessage() {
    this.Message = null;
  }
}
