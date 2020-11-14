import { Component } from '@angular/core';
import * as _ from 'lodash';
import { GenericResponse } from '../../../models/genericresponse'
import { SessionLandingResponse } from '../../../models/session'
import { Router } from '@angular/router';
import { LetterJamComponentBase } from '../../../models/letterjam/content';
import { GlobalSessionService } from '../../../services/session';

@Component({
  selector: 'letterjam-landing-component',
  templateUrl: './landing.component.html'
})
export class LetterJamLandingComponent extends LetterJamComponentBase {

  DisableButtons: boolean;

  SessionIdentifier: string;
  constructor(private _sessionService: GlobalSessionService, private _router: Router) {
    super();

    this.Loading = false;
  }

  public EnterGame() {
    this.clearMessage();
    var password = this.SessionIdentifier;
    if (!password) {
      this.ErrorMessage = "Please enter a game identifier";
      return;
    }

    this.DisableButtons = true;

    if (this.ReadSessionData(this.SessionIdentifier)) {
      this._router.navigate(['/letterjam', this.SessionIdentifier]);
    }
    else {
      this._sessionService.enterGame({ password: password })
        .then(data => this.handleResponse(data))
        .catch(data => this.HandleGenericError(data))
        .finally(() => this.DisableButtons = false);
    }
  }

  private handleResponse(data: GenericResponse<SessionLandingResponse>) {
    if (data.success === true) {
      this.clearMessage();

      this.SetSessionData(data.data.sessionId, data.data.playerId, this.SessionIdentifier);
      this._router.navigate(['/letterjam', this.SessionIdentifier]);
    }
    else {
      //Stay on page and display error
      this.ErrorMessage = data.errorCode;
    }
  }

  clearMessage() {
    this.ErrorMessage = null;
  }
}
