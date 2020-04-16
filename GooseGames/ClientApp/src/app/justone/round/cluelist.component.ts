import * as _ from 'lodash';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JustOnePlayerStatusService } from '../../../services/justone/playerstatus'
import { PlayerStatus } from '../../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../../models/justone/session';
import { GenericResponse, GenericResponseBase } from '../../../models/genericresponse';
import { PlayerCluesResponse, PlayerClue } from '../../../models/justone/clue';
import { PlayerNumberCss } from '../../../services/justone/ui'
import { TristateSwitch, ITristateSwitchHandler } from '../../../assets/tristate-switch.component'

export interface IJustOneClueListComponent extends IPlayerSessionComponent {
  preValidate() : Promise<any>;
  getPlayerStatus(): PlayerStatus;
  isClueListReadOnly(): boolean;
  loadClues(): Promise<GenericResponse<PlayerCluesResponse>>;
  loadContent(): Promise<any>;
  MarkClueAs(clue: PlayerClue, valid: boolean);
  onRedirect();
  setClueListComponent(clueListComponent: JustOneClueListComponent);
  Loading: boolean;
}

export abstract class JustOneClueListComponentBase implements IJustOneClueListComponent {
  abstract getPlayerStatus(): PlayerStatus;
  abstract isClueListReadOnly(): boolean;
  abstract loadClues(): Promise<GenericResponse<PlayerCluesResponse>>;
  abstract loadContent(): Promise<GenericResponseBase>;
  abstract MarkClueAs(clue: PlayerClue, valid: boolean);
  onRedirect() { };
  abstract setClueListComponent(clueListComponent: JustOneClueListComponent);
  preValidate(): Promise<any> { return Promise.resolve(); }

  constructor(activatedRoute: ActivatedRoute) {
    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;
  }

  HandleGenericError() {
    this.ErrorMessage = "An unknown error occurred";
  }

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;
}

@Component({
  selector: 'app-just-one-cluelist-component',
  templateUrl: './cluelist.component.html',
  styleUrls: ['./cluelist.component.css', '../sessionlobby.component.css']
})
export class JustOneClueListComponent implements OnInit {   

  PlayerNumberCss = PlayerNumberCss;

  private _playerStatusService: JustOnePlayerStatusService;

  Clues: PlayerClue[];
  ReadOnly = true;

  @Input() clueListComponent: IJustOneClueListComponent;
  Validate(): any {
    throw new Error("Method not implemented.");
  }
  ngOnInit() {
    this.clueListComponent.setClueListComponent(this);
    this.ReadOnly = this.clueListComponent.isClueListReadOnly();

    this.clueListComponent.preValidate()
      .then(() =>
      {
        return this.ValidateStatus()
      })    
      .then(response => {
        if (response.success) {
          return this.clueListComponent.loadClues();
        }
      })
      .then(response => {
        if (response && response.success) {
          this.Clues = response.data.responses;
        }
        else if (response) {
          this.clueListComponent.ErrorMessage = response.errorCode;
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {
          return this.clueListComponent.loadContent();
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {
          this.clueListComponent.Loading = false;
        }
      })
      .catch(() => {
        this.clueListComponent.HandleGenericError();
      });
  }

  constructor(playerStatusService: JustOnePlayerStatusService) {
    this._playerStatusService = playerStatusService;
  }


  ValidateStatus() {
      return this._playerStatusService.Validate(this.clueListComponent, this.clueListComponent.getPlayerStatus(), () => {
          this.clueListComponent.onRedirect();
      });
  }

  ValiditySwitchHandler(clue: PlayerClue): ITristateSwitchHandler {
    return {
      SwitchOff: () => this.MarkClueAsInvalid(clue),
      SwitchOn: () => this.MarkClueAsValid(clue),
      SwitchUnselected: () => { },
      DefaultState: clue.responseInvalid ? false : null,
      AllowUnselected: false,
      ReadOnly: clue.responseInvalid === true,
      GroupName: "clue-valid-switch-"
    }
  }

  MarkClueAsInvalid(clue: PlayerClue) {
    this.clueListComponent.MarkClueAs(clue, false);
  }
  MarkClueAsValid(clue: PlayerClue) {
    this.clueListComponent.MarkClueAs(clue, true);
  }
}
