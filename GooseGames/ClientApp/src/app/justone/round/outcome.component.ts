import { Component } from '@angular/core';
import * as _ from 'lodash';
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../../models/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { IPlayerSessionComponent } from '../../../models/session';
import { PlayerStatus, PlayerStatusRoutesMap } from '../../../models/justone/playerstatus';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus';
import { GenericResponseBase, GenericResponse } from '../../../models/genericresponse';
import { RoundOutcomeResponse, RoundInformationResponse } from '../../../models/justone/round';
import { JustOneRoundService } from '../../../services/justone/round';
import { NavbarService } from '../../../services/navbar';
import { PlayerNumberCss } from '../../../services/justone/ui'
import { NavbarHeaderEnum } from '../../nav-menu/navbar-header';
import { JustOneClueListComponentBase, JustOneClueListComponent } from './cluelist.component';
import { PlayerCluesResponse, PlayerClue } from '../../../models/justone/clue';
import { JustOneClueService } from '../../../services/justone/clue';
import { JustOneLocalStorage } from '../../../services/justone/localstorage';

export abstract class JustOneRoundOutcomeComponentBase extends JustOneClueListComponentBase {


  PlayerNumberCss = PlayerNumberCss;

  _router: Router;
  _playerStatusService: JustOnePlayerStatusService;
  _roundService: JustOneRoundService;
  _navbarService: NavbarService;
  _clueService: JustOneClueService;
  _clueListComponent: JustOneClueListComponent;
  _justOneLocalStorage: JustOneLocalStorage;

  SessionId: string;
  PlayerId: string;
  Loading: boolean = true;
  ErrorMessage: string;

  RoundOutcome: RoundOutcomeResponse;

  constructor(playerStatusService: JustOnePlayerStatusService,
    roundService: JustOneRoundService,
    navbarService: NavbarService,
    clueService: JustOneClueService,
    justOneLocalStorage: JustOneLocalStorage,
    router: Router,
    activatedRoute: ActivatedRoute) {

    super(activatedRoute);

    this._router = router;
    this._playerStatusService = playerStatusService;
    this._roundService = roundService;
    this._navbarService = navbarService;
    this._clueService = clueService;
    this._justOneLocalStorage = justOneLocalStorage;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    //this._playerStatusService.Validate(this, this.getPlayerStatus(), () => { })
    //  .then(data => {
    //    if (data.success) {
    //      return this.load();
    //    }
    //  })
    //  .then(data => {
    //    if (data && data.success) {
    //      this.Loading = false;
    //    }
    //  })
    //  .catch(() => this.HandleGenericError());
  }

  abstract isActivePlayer(): boolean;

  updateCurrentRoundInformation() {
    var currentRoundInformation;
    var roundInformationString = localStorage.getItem('just-one-navbar-round-info');
    if (roundInformationString) {
      currentRoundInformation = <RoundInformationResponse>JSON.parse(roundInformationString);
    }
    else {
      currentRoundInformation = <RoundInformationResponse>{
        roundNumber: 1
      }
    }
    currentRoundInformation.score = this.RoundOutcome.score;
    currentRoundInformation.roundsTotal = this.RoundOutcome.nextRoundInformation.roundsTotal;

    localStorage.setItem('just-one-navbar-round-info', JSON.stringify(currentRoundInformation));

    this._navbarService.setAreaContent(NavbarHeaderEnum.JustOne);
  }

  NextRound() {
    this._playerStatusService
      .SetRoundWaiting(
        {
          SessionId: this.SessionId,
          PlayerId: this.PlayerId,
          RoundId: this.RoundOutcome.roundId
        })
      .then(response => {
        if (response.success) {

          localStorage.setItem('just-one-navbar-round-info', JSON.stringify(this.RoundOutcome.nextRoundInformation));
          this._navbarService.setAreaContent(NavbarHeaderEnum.JustOne);

          this._router.navigate([
            PlayerStatusRoutesMap.RoundWaiting, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
        else {
          this.ErrorMessage = response.errorCode;
        }
      })
      .catch(() => this.HandleGenericError());
  }

  Again = () => {
    this._playerStatusService.Set(this.PlayerId, this.SessionId, PlayerStatus.InLobby)
      .then(response => {
        if (response.success) {
          this._navbarService.reset();
        this._router.navigate([
          PlayerStatusRoutesMap.InLobby, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
      }
    });
  }

  isClueListReadOnly(): boolean {
    return true;
  }
  loadClues(): Promise<GenericResponse<PlayerCluesResponse>> {
    return this._clueService.GetClues(this);
  }
  loadContent(): Promise<GenericResponseBase> {
    return this._roundService.GetRoundOutcome({
      SessionId: this.SessionId,
      PlayerId: this.PlayerId
    })
      .then(response => {
        if (response.success) {
          this.RoundOutcome = response.data;
          if (this.isActivePlayer()) {
            this.RoundOutcome.activePlayerName = "You";
          }

          if (this.RoundOutcome.gameEnded) {
            this._navbarService.setReadOnly(false);
            this._justOneLocalStorage.ClearPlayerDetails();
          }
          else {
            this._justOneLocalStorage.CachePlayerDetails(this);
          }
          this.updateCurrentRoundInformation();
        }
        else {
          this.ErrorMessage = response.errorCode;
        }
        return response;
      });
  }

  abstract getPlayerStatus(): PlayerStatus;

  MarkClueAs(clue: PlayerClue, valid: boolean) {

  }
  onRedirect() {

  }
  setClueListComponent(clueListComponent: JustOneClueListComponent) {
    this._clueListComponent = clueListComponent;
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}


@Component({
  selector: 'app-just-one-activeplayerroundoutcome-component',
  templateUrl: './outcome.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOneActivePlayerRoundOutcomeComponent extends JustOneRoundOutcomeComponentBase {  

  constructor(playerStatusService: JustOnePlayerStatusService, roundService: JustOneRoundService, navbarService: NavbarService, clueService: JustOneClueService,
    justOneLocalStorage: JustOneLocalStorage, router: Router, activatedRoute: ActivatedRoute) {
    super(playerStatusService, roundService, navbarService, clueService, justOneLocalStorage, router, activatedRoute);
  }
  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.ActivePlayerOutcome;
  }
  isActivePlayer(): boolean {
    return true;
  }
}

@Component({
  selector: 'app-just-one-passiveplayerroundoutcome-component',
  templateUrl: './outcome.component.html',
  styleUrls: ['../sessionlobby.component.css']
})
export class JustOnePassivePlayerRoundOutcomeComponent extends JustOneRoundOutcomeComponentBase {

  constructor(playerStatusService: JustOnePlayerStatusService, roundService: JustOneRoundService, navbarService: NavbarService, clueService: JustOneClueService,
    justOneLocalStorage: JustOneLocalStorage, router: Router, activatedRoute: ActivatedRoute) {
    super(playerStatusService, roundService, navbarService, clueService, justOneLocalStorage, router, activatedRoute);
  }
  getPlayerStatus(): PlayerStatus {
    return PlayerStatus.PassivePlayerOutcome;
  }
  isActivePlayer(): boolean {
    return false;
  }
}
