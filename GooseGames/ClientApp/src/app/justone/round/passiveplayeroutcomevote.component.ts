import { Component } from '@angular/core';
import * as _ from 'lodash';
import { JustOnePlayerDetailsService } from '../../../services/justone/playerdetails'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../../models/justone/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { IPlayerSessionComponent } from '../../../models/session';
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus';
import { GenericResponseBase, GenericResponse } from '../../../models/genericresponse';
import { JustOneClueService } from '../../../services/justone/clue';
import { PlayerClue, SubmitClueVotesRequest, PlayerCluesResponse } from '../../../models/justone/clue';
import { PlayerNumberCss } from '../../../services/justone/ui'

@Component({
  selector: 'app-just-one-passiveplayeroutcomevote-component',
  templateUrl: './passiveplayeroutcomevote.component.html'
})

export class JustOnePassivePlayerOutcomeVoteComponent implements IPlayerSessionComponent {
  PlayerNumberCss = PlayerNumberCss;
  _router: Router;
  _playerStatusService: JustOnePlayerStatusService;
  _clueService: JustOneClueService;

  SessionId: string;
  PlayerId: string;  
  Loading: boolean = true;
  ErrorMessage: string;

  ActivePlayerName: string;
  ActivePlayerNumber: number;
  ResponseWord: string;
  WordToGuess: string;

  _responseId: string;

  constructor(playerStatusService: JustOnePlayerStatusService, clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {
    this._router = router;
    this._playerStatusService = playerStatusService;
    this._clueService = clueService;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this._playerStatusService.Validate(this, PlayerStatus.PassivePlayerOutcomeVote, () => { })
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

  VoteCorrect() {
    this.vote({
      PlayerId: this.PlayerId,
      SessionId: this.SessionId,
      ValidResponses: [this._responseId]
    });
  }

  VoteIncorrect() {
    this.vote({
      PlayerId: this.PlayerId,
      SessionId: this.SessionId,
      ValidResponses: []
    });
  }

  vote(request: SubmitClueVotesRequest) {
    this._clueService.SubmitActivePlayerResponseVote(request)
      .then(response => {
        if (response.success) {
          this._router.navigate([
            PlayerStatusRoutesMap.PassivePlayerWaitingForOutcomeVotes, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
        else {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(() => this.HandleGenericError());
  }

  load(): Promise<void | GenericResponse<PlayerCluesResponse>> {

    return this._clueService.GetActivePlayerResponse(this)
      .then(response => {
        if (response.success) {
          this.ActivePlayerName = response.data.activePlayerName;
          this.ActivePlayerNumber = response.data.activePlayerNumber;
          this.WordToGuess = response.data.wordToGuess;
          this.ResponseWord = response.data.responses[0].response;
          this._responseId = response.data.responses[0].responseId;
        }
        else {
          this.ErrorMessage = response.errorCode;
        }
        return response;
      })
      .catch(() => this.HandleGenericError());
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
