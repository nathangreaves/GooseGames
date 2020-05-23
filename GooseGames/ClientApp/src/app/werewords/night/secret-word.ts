import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsPlayerStatus, WerewordsComponentBase } from '../../../models/werewords/content';
import { WerewordsRoundService } from '../../../services/werewords/round';
import { GenericResponseBase, GenericResponse } from '../../../models/genericresponse';
import { SecretRole } from '../../../models/werewords/playerroundinformation';
import { WerewordsPlayerStatusService } from '../../../services/werewords/playerstatus';

@Component({
  selector: 'app-werewords-night-secret-word-component',
  templateUrl: './secret-word.html',
  styleUrls: ['./secret.css']
})
export class WerewordsNightSecretWordComponent extends WerewordsComponentBase implements OnInit {
  SecretRole: SecretRole;
  MayorName: string;
  MayorId: string;
  IsMayor: boolean;
  DisableButtons: boolean = true;
  WaitingForMayor: boolean = false;
  SecretWord: string;
  SecretWordHidden: boolean = true;

  constructor(private roundService: WerewordsRoundService, private playerStatusService: WerewordsPlayerStatusService) {
    super();
  }

  ngOnInit(): void {

    this.load()
      .then(() =>
      {
        this.Loading = false;
      });
  }

  load(): Promise<any> {
    return this.roundService.GetSecretWord(this).then(response => this.HandleGenericResponse(response, data =>
    {
      this.SecretRole = data.secretRole;
      this.MayorName = data.mayorName;
      this.MayorId = data.mayorPlayerId;
      this.IsMayor = data.mayorPlayerId.toLowerCase() == this.PlayerId.toLowerCase();
      this.SecretWord = data.secretWord;

      return Promise.resolve(<GenericResponseBase>{ success: true })
    }));
  }

  WakeUp() {
    this.DisableButtons = true;

    this.playerStatusService.TransitionToNextNightStatus(this).then(response => this.HandleGenericResponse(response, data => {
      this.Route(data);
      return Promise.resolve(<GenericResponseBase>{ success: true });
    }));

  }
  show() {
    this.SecretWordHidden = false;
    this.DisableButtons = false;
  }

  hide() {
    this.SecretWordHidden = true;
  }

}
