import { Component } from '@angular/core';
import * as _ from 'lodash';
import { RoundInformationResponse } from '../../models/justone/round';
import { INavbarHeaderComponent } from '../nav-menu/navbar-header-content';

@Component({
  selector: 'app-just-one-navbarheader-component',
  templateUrl: './navbarheader.component.html',
  styleUrls: ['./navbarheader.component.css']
})
export class JustOneNavbarHeaderComponent implements INavbarHeaderComponent {

  ErrorMessage: string;
  RoundInfo: RoundInformationResponse;

  constructor() {

    var roundInfoJson = localStorage.getItem('just-one-navbar-round-info');
    if (roundInfoJson) {
      this.RoundInfo = <RoundInformationResponse>JSON.parse(roundInfoJson);
    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
