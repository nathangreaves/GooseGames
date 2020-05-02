import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOneSessionService } from '../../services/justone/session'
import { GenericResponse } from '../../models/genericresponse'
import { SessionLandingResponse } from '../../models/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { NavbarService } from '../../services/navbar';
import { JustOneLocalStorage } from '../../services/justone/localstorage';

@Component({
  selector: 'app-just-one-landing-component',
  templateUrl: './landing.component.html'
})

export class JustOneLandingComponent {

  _sessionService: JustOneSessionService;
  _justOneLocalStorage: JustOneLocalStorage;
  _router: Router;
  _navbarService: NavbarService;

  ErrorMessage: string;
  DisableButtons: boolean;
  CanRejoin: boolean = false;

  constructor(sessionService: JustOneSessionService, justOneLocalStorage: JustOneLocalStorage, navbarService: NavbarService, router: Router) {
    this._sessionService = sessionService;
    this._navbarService = navbarService;
    this._justOneLocalStorage = justOneLocalStorage;
    this._router = router;

    this._navbarService.setAreaTitle("Just One");
    localStorage.removeItem('just-one-navbar-round-info');

    if (this._justOneLocalStorage.GetPlayerDetails()) {
      this.CanRejoin = true;
    }
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

  public RejoinExistingGame() {
    this._router.navigate(['/justone/rejoin']);
  }

  private handleResponse(data: GenericResponse<SessionLandingResponse>) {
    if (data.success === true) {
      this.clearMessage();

      this._navbarService.setReadOnly(true);

      this._router.navigate(['/justone/disclaimer', { SessionId: data.data.sessionId, PlayerId: data.data.playerId }]);
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
