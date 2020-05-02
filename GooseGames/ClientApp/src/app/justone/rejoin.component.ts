import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { IPlayerSessionComponent } from '../../models/session';
import { PlayerStatus } from '../../models/justone/playerstatus';
import { JustOnePlayerStatusService } from '../../services/justone/playerstatus';
import { GenericResponseBase } from '../../models/genericresponse';
import { JustOneLocalStorage } from '../../services/justone/localstorage';

@Component({
  selector: 'app-just-one-rejoin-component',
  templateUrl: './rejoin.component.html'
})

export class JustOneRejoinComponent implements IPlayerSessionComponent {

  _playerStatusService: JustOnePlayerStatusService;
  _justOneLocalStorage: JustOneLocalStorage;

  SessionId: string;
  PlayerId: string;
  Loading: boolean = true;
  ErrorMessage: string;

  constructor(justOneLocalStorage: JustOneLocalStorage, playerStatusService: JustOnePlayerStatusService) {
    this._playerStatusService = playerStatusService;
    this._justOneLocalStorage = justOneLocalStorage;

    var playerDetails = this._justOneLocalStorage.GetPlayerDetails();

    if (playerDetails) {

      this.SessionId = playerDetails.SessionId;
      this.PlayerId = playerDetails.PlayerId;

      this._playerStatusService.Validate(this, PlayerStatus.Rejoining, () => { });
    }
  }

  load(): Promise < GenericResponseBase > {
    return Promise.resolve({ success: true, errorCode: null });
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
