import { Component } from '@angular/core';
import * as _ from 'lodash';
import { FujiSessionService } from '../../services/fujiflush/session'
import { GenericResponse } from '../../models/genericresponse'
import { SessionLandingResponse } from '../../models/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { NavbarService } from '../../services/navbar';
import { FujiLocalStorage } from '../../services/fujiflush/localstorage';
import { GlobalSessionService } from '../../services/session';

@Component({
  selector: 'app-fuji-landing-component',
  templateUrl: './landing.component.html'
})

export class FujiLandingComponent {

  _fujiLocalStorage: FujiLocalStorage;
  _router: Router;
  _navbarService: NavbarService;

  ErrorMessage: string;
  DisableButtons: boolean;
  CanRejoin: boolean = false;

  constructor(private sessionService: GlobalSessionService, fujiLocalStorage: FujiLocalStorage, navbarService: NavbarService, router: Router) {
      this._navbarService = navbarService;
    this._fujiLocalStorage = fujiLocalStorage;
    this._router = router;

    this._navbarService.reset();
    this._navbarService.setAreaTitle("Fuji Flush");

    if (this._fujiLocalStorage.GetPlayerDetails()) {
      this.CanRejoin = true;
    }
  }

  public JoinGame(password: string) {
    this.clearMessage();

    if (!password) {
      this.ErrorMessage = "No game identifier given for joining game";
      return;
    }

    this.DisableButtons = true;
    this.sessionService.enterGame({ password: password })
      .then(data => this.handleResponse(data))
      .catch(data => this.genericError())
      .finally(() => this.DisableButtons = false);
  }

  public RejoinExistingGame() {
    this._router.navigate(['/fujiflush/rejoin']);
  }

  private handleResponse(data: GenericResponse<SessionLandingResponse>) {
    if (data.success === true) {
      this.clearMessage();

      this._navbarService.setReadOnly(true);

      this._router.navigate(['/fujiflush/sessionlobby', { SessionId: data.data.sessionId, PlayerId: data.data.playerId }]);
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
