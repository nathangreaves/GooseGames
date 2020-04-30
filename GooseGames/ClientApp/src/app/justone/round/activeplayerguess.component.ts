import { Component } from '@angular/core';
import * as _ from 'lodash';
import * as signalR from "@microsoft/signalr";
import { Router, ActivatedRoute } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { JustOneClueService } from '../../../services/justone/clue';
import { JustOneClueListComponentBase, JustOneClueListComponent } from './cluelist.component';
import { GenericResponse, GenericResponseBase } from '../../../models/genericresponse';
import { PlayerCluesResponse } from '../../../models/justone/clue';

@Component({
  selector: 'app-just-one-activeplayerguess-component',
  templateUrl: './activeplayerguess.component.html',
  styleUrls: ['./activeplayerguess.component.css', './cluelist.component.css']
})

export class JustOneActivePlayerGuess extends JustOneClueListComponentBase {

  _router: Router;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  DisableButtons: boolean;

  public Loading: boolean;
  _clueService: JustOneClueService;
  _clueListComponent: JustOneClueListComponent;

  constructor(clueService: JustOneClueService, router: Router, activatedRoute: ActivatedRoute) {

    super(activatedRoute);
    this._router = router;
    this._clueService = clueService;
  }

  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.ActivePlayerGuess;
  }
  isClueListReadOnly(): boolean {
    return true;
  }
  loadClues(): Promise<GenericResponse<PlayerCluesResponse>> {
    return this._clueService.GetClues(this);
  }
  loadContent(): Promise<GenericResponseBase> {
    return Promise.resolve(
      {
        success: true,
        errorCode: null,
      });
  }
  MarkClueAs() {
  }
  setClueListComponent(clueListComponent: JustOneClueListComponent) {
    this._clueListComponent = clueListComponent;
  }

  SubmitResponse(response: string) {
    if (!response) {
      this.ErrorMessage = "Please submit response";
      return;
    }
    if (response.trim().indexOf(" ") != -1) {
      this.ErrorMessage = "Please submit JUST ONE word";
      return;
    }

    this.DisableButtons = true;
    this._clueService.SubmitActivePlayerResponse({
      SessionId: this.SessionId,
      PlayerId: this.PlayerId,
      ResponseWord: response.trim(),
      Pass: false
    })
      .then(response => {
        this.HandleResponse(response);
      })
      .catch(() => this.HandleGenericError())
      .finally(() => this.DisableButtons = false);
  }

  SubmitPass(response: string) {
    this.DisableButtons = true;
    this._clueService.SubmitActivePlayerResponse({
      SessionId: this.SessionId,
      PlayerId: this.PlayerId,
      ResponseWord: response,
      Pass: true
    })
      .then(response => {
        this.HandleResponse(response);
      })
      .catch(() => this.HandleGenericError())
      .finally(() => this.DisableButtons = false);
  }

  HandleResponse(response: GenericResponseBase): Promise<GenericResponseBase> {
    if (!response.success) {
      this.ErrorMessage = response.errorCode;
    }
    else {
      return this._clueListComponent.ValidateStatus();
    }
    return Promise.resolve(response);
  }
}
