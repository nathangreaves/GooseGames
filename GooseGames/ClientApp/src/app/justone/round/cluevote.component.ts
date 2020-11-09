import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus';
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus';
import { JustOneClueService } from '../../../services/justone/clue';
import { IPlayerSessionComponent } from '../../../models/session';
import { PlayerClue, PlayerCluesResponse } from '../../../models/justone/clue';
import { JustOneClueListComponentBase, JustOneClueListComponent } from './cluelist.component';
import { GenericResponse, GenericResponseBase } from '../../../models/genericresponse';
import { PlayerNumberCss } from '../../../services/justone/ui'

@Component({
  selector: 'app-just-one-cluevote-component',
  templateUrl: './cluevote.component.html',
  styleUrls: ['./cluevote.component.css', '../sessionlobby.component.css']
})
export class JustOneClueVoteComponent extends JustOneClueListComponentBase {
  
  PlayerNumberCss = PlayerNumberCss;
  _router: Router;
  _playerStatusService: JustOnePlayerStatusService;
  _clueService: JustOneClueService;

  ActivePlayerNumber: number;
  ActivePlayerName: string;
  ActivePlayerEmoji: string;
  Word: string;
  RevealedWord: string;
  clueListComponent: JustOneClueListComponent;
  DisableSubmitClueVote: boolean;
  
  constructor(clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {

    super(activatedRoute);

    this._router = router;
    this._clueService = clueService;

    this.hide();
  }
  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.PassivePlayerClueVote;
  }
  isClueListReadOnly(): boolean {
    return false;
  }

  show() {
    this.Word = this.RevealedWord;
  }

  hide() {
    this.Word = "PRESS TO REVEAL";
  }

  loadClues(): Promise<GenericResponse<PlayerCluesResponse>> {
    return this._clueService.GetClues(this).then(response => {
      if (response.success) {
        this.ActivePlayerNumber = response.data.activePlayerNumber;
        this.ActivePlayerName = response.data.activePlayerName;
        this.ActivePlayerEmoji = response.data.activePlayerEmoji;
        this.RevealedWord = response.data.wordToGuess;

        _.forEach(response.data.responses, clue => { clue.responseVoted = (clue.responseInvalid === true) ? false : null; });
      }
      return response;
    });
  }
  loadContent(): Promise<GenericResponseBase> {
    return Promise.resolve(
      {
        success: true,
        errorCode: null,
      });
  }
  onRedirect() {

  }
  setClueListComponent(clueListComponent: JustOneClueListComponent) {
    this.clueListComponent = clueListComponent;
  }

  MarkClueAs(clue: PlayerClue, valid: boolean) {

    if (valid === true) {
      clue.responseVoted = true;
    }
    else if (valid === false) {
      clue.responseVoted = false;
    }
    else {
      clue.responseVoted = null;
    }
  }

  HandleGenericError() {
    this.DisableSubmitClueVote = false;
    super.HandleGenericError();
  }

  SubmitClueVote() {

    this.DisableSubmitClueVote = true;
    if (_.find(this.clueListComponent.Clues, clue => clue.responseVoted == null)) {
      this.ErrorMessage = "Please mark all responses as either Valid or Invalid";
      this.DisableSubmitClueVote = false;
      return;
    }

    this._clueService.SubmitClueVote(
      {
        SessionId: this.SessionId,
        PlayerId: this.PlayerId,
        ValidResponses: _.map(_.filter(this.clueListComponent.Clues, clue => clue.responseVoted), clue => clue.responseId)
      })
      .then(data => {
        if (!data.success) {
          this.ErrorMessage = data.errorCode;
          this.DisableSubmitClueVote = false;
        }
        else {
          this._router.navigate([
            PlayerStatusRoutesMap.PassivePlayerWaitingForClueVotes, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
      })
      .catch(() => this.HandleGenericError());
  }
}
