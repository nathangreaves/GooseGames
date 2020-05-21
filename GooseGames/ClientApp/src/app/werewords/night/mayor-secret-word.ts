import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsPlayerStatus, WerewordsComponentBase } from '../../../models/werewords/content';
import { WerewordsRoundService } from '../../../services/werewords/round';
import { GenericResponseBase } from '../../../models/genericresponse';

@Component({
  selector: 'app-werewords-night-mayor-secret-word-component',
  templateUrl: './mayor-secret-word.html'
})
export class WerewordsNightMayorSecretWordComponent extends WerewordsComponentBase implements OnInit {

  Words: string[];
  DisableButtons: boolean = false;

  constructor(private werewordsRoundService: WerewordsRoundService) {
    super();
  }

  ngOnInit(): void {

    this.werewordsRoundService.GetWords(this)
      .then(response => this.HandleGenericResponse(response, data => {

        this.Words = data;

        return Promise.resolve(<GenericResponseBase>{
          success: true
        });
      }));

  }

  SelectWord(word: string) {

    this.DisableButtons = true;

    this.werewordsRoundService.SelectWord(this, word)
      .then(response => this.HandleGenericResponseBase(response, () => {

        this.Route(WerewordsPlayerStatus.NightSecretWord);
        return Promise.resolve(response);
      }))
      .finally(() => {
        this.DisableButtons = false;
      });
  }
}
