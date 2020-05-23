import { Component } from '@angular/core';
import * as _ from 'lodash';
import { GenericResponse } from '../../../models/genericresponse'
import { SessionLandingResponse } from '../../../models/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { WerewordsComponentBase, WerewordsPlayerStatus } from '../../../models/werewords/content';
import { WerewordsSessionService } from '../../../services/werewords/session';
import { NavbarService } from '../../../services/navbar';

@Component({
  selector: 'app-werewords-landing-component',
  templateUrl: './landing.html'
})

export class WerewordsLandingComponent extends WerewordsComponentBase {

  DisableButtons: boolean;

  SessionIdentifier: string;
  constructor(private _sessionService: WerewordsSessionService, private _router: Router) {
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
    this._sessionService.EnterGame({ password: password })
      .then(data => this.handleResponse(data))
      .catch(data => this.HandleGenericError(data))
      .finally(() => this.DisableButtons = false);
  }

  private handleResponse(data: GenericResponse<SessionLandingResponse>) {
    if (data.success === true) {
      this.clearMessage();

      this.SetSessionData(data.data.sessionId, data.data.playerId);
      this._router.navigate(['/werewords', this.SessionIdentifier]);
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
