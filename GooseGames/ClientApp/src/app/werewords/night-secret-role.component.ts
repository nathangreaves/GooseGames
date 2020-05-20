import { Component } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsContentEnum, WerewordsComponentBase } from '../../models/werewords/content';

@Component({
  selector: 'app-werewords-night-secret-role-component',
  templateUrl: './night-secret-role.component.html'
})
export class WerewordsNightSecretRoleComponent extends WerewordsComponentBase {
    
  constructor() {

    super();

  }

  ChangePage() {
    this.Route(WerewordsContentEnum.NightSecretWord);
  }
}
