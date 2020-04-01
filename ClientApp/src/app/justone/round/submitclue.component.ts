import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOneRoundService } from '../../../services/justone/round'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { GenericResponse } from '../../../models/genericresponse';
import { JustOneClueService } from '../../../services/justone/clue';

@Component({
  selector: 'app-just-one-submitclue-component',
  templateUrl: './submitclue.component.html'
})

export class JustOneSubmitClueComponent implements IPlayerSessionComponent {

  _router: Router;
  _playerStatusService: JustOnePlayerStatusService;
  _roundService: JustOneRoundService;
  _clueService: JustOneClueService;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;

  ActivePlayerNumber: number;
  ActivePlayerName: string;
  Word: string;
  RevealedWord: string;

  constructor(playerStatusService: JustOnePlayerStatusService, roundService: JustOneRoundService, clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {
    this._router = router;
    this._playerStatusService = playerStatusService;
    this._roundService = roundService;
    this._clueService = clueService;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.hide();

    this._playerStatusService.Validate(this, PlayerStatus.PassivePlayerClue, () => { })
      .then(data => {
        if (data.success) {
          return this.load();
        }
      })
      .then(data => {
        if (data && data.success) {
          this.Loading = false;
        }
      });
  }

  show() {
    this.Word = this.RevealedWord;
  }

  hide() {
    this.Word = "PRESS TO REVEAL";
}

  load(): Promise<any | GenericResponse<any>> {
    return this._roundService.GetRoundForPassivePlayer(this)
      .then(response => {
        if (response.success) {
          this.ActivePlayerNumber = response.data.activePlayerNumber;
          this.ActivePlayerName = response.data.activePlayerName;
          this.RevealedWord = response.data.word;
        }
        else {
          this.ErrorMessage = response.errorCode;
        }
        return response;
      })
      .catch(() => {
        this.HandleGenericError();
      });
  }

  SubmitClue(clue: string) {
    this._clueService.SubmitClue({
      ResponseWord: clue,
      PlayerId: this.PlayerId,
      SessionId: this.SessionId
    })
      .then(response => {
        if (!response.success) {
          this.ErrorMessage = response.errorCode;
        }
        else {
          this._router.navigate([
            PlayerStatusRoutesMap.PassivePlayerWaitingForClues, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
      })
      .catch(() => this.HandleGenericError());
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}