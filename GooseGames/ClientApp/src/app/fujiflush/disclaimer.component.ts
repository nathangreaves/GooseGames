import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-fuji-disclaimer-component',
  templateUrl: './disclaimer.component.html'
})

export class FujiDisclaimerComponent {

  _router: Router;

  SessionId: string;
  PlayerId: string;

  constructor(router: Router, activatedRoute: ActivatedRoute) {
    this._router = router;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;
  }

  Continue() {
    this._router.navigate(['/fujiflush/newplayer', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
  }
}
