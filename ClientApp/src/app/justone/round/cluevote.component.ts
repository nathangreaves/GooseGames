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

  //private _hubConnection: signalR.HubConnection;

  constructor(clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {

    super(activatedRoute);

    this._router = router;
    this._clueService = clueService;

    //this.SessionId = activatedRoute.snapshot.params.SessionId;
    //this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    //this.setupConnection();

    //this._playerStatusService.Validate(this, PlayerStatus.PassivePlayerClueVote, () => { })
    //  .then(data => {
    //    if (data.success) {
    //      return this.load();
    //    }
    //  })
    //  .then(data => {
    //    if (data && data.success) {
    //      this.Loading = false;
    //    }
    //  });
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

  //load(): Promise<any> {
  //  return this._clueService.GetClues(this)
  //    .then(response => {
  //      if (response.success) {

  //        this.ActivePlayerNumber = response.data.activePlayerNumber;
  //        this.ActivePlayerName = response.data.activePlayerName;
  //        this.Word = response.data.wordToGuess;

  //        _.forEach(response.data.responses, clue => clue.responseAutoInvalid = clue.responseInvalid);
  //        this.Clues = response.data.responses;
  //      }
  //      else {
  //        this.ErrorMessage = response.errorCode;
  //      }
  //      return response;
  //    })
  //    .catch(() => this.HandleGenericError());
  //}

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

  //private setupConnection() {
  //this._hubConnection = new signalR.HubConnectionBuilder()
  //  .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
  //  .build();

  //this._hubConnection.on(*"message"*, () => {
  //  this._hubConnection.stop();
  //  this._hubConnection = null;
  //  this._router.navigate(['/justone/submitclue', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
  //});
  //this._hubConnection.on("beginRoundActivePlayer", () => {
  //  this._hubConnection.stop();
  //  this._hubConnection = null;
  //  this._router.navigate(['/justone/playerwaiting', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
  //});

  //this._hubConnection.start().catch(err => console.error(
  //}
}
