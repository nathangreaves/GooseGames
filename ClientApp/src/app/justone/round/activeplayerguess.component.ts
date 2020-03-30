import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';

@Component({
  selector: 'app-just-one-activeplayerguess-component',
  templateUrl: './activeplayerguess.component.html'
})

export class JustOneActivePlayerGuess implements IPlayerSessionComponent {

  _playerStatusService: JustOnePlayerStatusService;
  _router: Router;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;

  public Loading: boolean;

  constructor(playerStatusService: JustOnePlayerStatusService, router: Router, activatedRoute: ActivatedRoute) {
    this._playerStatusService = playerStatusService;
    this._router = router;

    this.Loading = true;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this._playerStatusService.Validate(this, PlayerStatus.ActivePlayerGuess, () => { })
      .then(response => { if (response.success) { return this.load(); } })
      .catch(() => this.HandleGenericError());
  }

  private load() {
    return Promise.resolve();
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
