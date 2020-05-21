import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-codenames-landing-component',
  templateUrl: './landing.component.html'
})

export class CodenamesLandingComponent {

  _router: Router;

  GameIdentifier: string;

  constructor(router: Router) {
    this._router = router;
    //activatedRoute: ActivatedRoute

  }

  Go() {
    this._router.navigate(['/codenames/session', this.GameIdentifier]);
  }
}
