import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOneSessionService } from '../../services/justone/session'
import { GenericResponse } from '../../models/genericresponse'
import { SessionLandingResponse } from '../../models/justone/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { NavbarService } from '../../services/navbar';

@Component({
  selector: 'app-just-one-landing-component',
  templateUrl: './landing.component.html'
})

export class JustOneLandingComponent {

  _sessionService: JustOneSessionService;
  _router: Router;
  _navbarService: NavbarService;

  ErrorMessage: string;
  DisableButtons: boolean;

  constructor(sessionService: JustOneSessionService, navbarService: NavbarService, router: Router) {
    this._sessionService = sessionService;
    this._navbarService = navbarService;
    this._router = router;

    this._navbarService.setAreaTitle("Just One");
    localStorage.removeItem('just-one-navbar-round-info');
  }


  public StartNewGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.ErrorMessage = "No game identifier given for new game";
      return;
    }

    this.DisableButtons = true;
    this._sessionService.CreateGame({ password: password })
      .then(data => this.handleResponse(data))
      .catch(data => this.genericError())
      .finally(() => this.DisableButtons = false);

  }

  public JoinGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.ErrorMessage = "No game identifier given for joining game";
      return;
    }

    this.DisableButtons = true;
    this._sessionService.JoinGame({ password: password })
      .then(data => this.handleResponse(data))
      .catch(data => this.genericError())
      .finally(() => this.DisableButtons = false);
  }

  private handleResponse(data: GenericResponse<SessionLandingResponse>) {
    if (data.success === true) {
      this.clearMessage();

      this._navbarService.setReadOnly(true);

      this._router.navigate(['/justone/declaration', { SessionId: data.data.sessionId, PlayerId: data.data.playerId }]);
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
