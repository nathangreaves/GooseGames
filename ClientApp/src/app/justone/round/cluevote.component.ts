import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus';
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus';
import { JustOneClueService } from '../../../services/justone/clue';
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { PlayerClue, PlayerCluesResponse } from '../../../models/justone/clue';
import { JustOneClueListComponentBase, JustOneClueListComponent } from './cluelist.component';
import { GenericResponse, GenericResponseBase } from '../../../models/genericresponse';

@Component({
  selector: 'app-just-one-cluevote-component',
  templateUrl: './cluevote.component.html',
  styleUrls: ['./cluevote.component.css', '../sessionlobby.component.css']
})
export class JustOneClueVoteComponent extends JustOneClueListComponentBase {
  _router: Router;
  _playerStatusService: JustOnePlayerStatusService;
  _clueService: JustOneClueService;

  ActivePlayerNumber: number;
  ActivePlayerName: string;
  Word: string;
  clueListComponent: JustOneClueListComponent;

  constructor(clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {

    super(activatedRoute);

    this._router = router;
    this._clueService = clueService;
  }

  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.PassivePlayerClueVote;
  }
  isClueListReadOnly(): boolean {
    return false;
  }
  loadClues(): Promise<GenericResponse<PlayerCluesResponse>> {
    return this._clueService.GetClues(this).then(response => {
      if (response.success) {
        this.ActivePlayerNumber = response.data.activePlayerNumber;
        this.ActivePlayerName = response.data.activePlayerName;
        this.Word = response.data.wordToGuess;
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

  MarkClueAsInvalid(clue: PlayerClue) {
    clue.responseInvalid = !clue.responseInvalid;
  }

  SubmitClueVote() {
    this._clueService.SubmitClueVote(
      {
        SessionId: this.SessionId,
        PlayerId: this.PlayerId,
        ValidResponses: _.map(_.filter(this.clueListComponent.Clues, clue => !clue.responseInvalid), clue => clue.responseId)
      })
      .then(data => {
        if (!data.success) {
          this.ErrorMessage = data.errorCode;
        }
        else {
          this._router.navigate([
            PlayerStatusRoutesMap.PassivePlayerWaitingForClueVotes, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
      })
      .catch(() => this.HandleGenericError());
  }
}
