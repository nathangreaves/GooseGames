import { Component } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsContentEnum, WerewordsComponentBase } from '../../models/werewords/content';

@Component({
  selector: 'app-werewords-night-secret-word-component',
  templateUrl: './night-secret-word.component.html'
})
export class WerewordsNightSecretWordComponent extends WerewordsComponentBase {
    
  constructor() {

    super();

  }

  ChangePage() {
    this.Route(WerewordsContentEnum.NightSecretRole);
  }
}
