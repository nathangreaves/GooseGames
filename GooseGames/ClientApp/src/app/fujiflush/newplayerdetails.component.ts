import { Component } from '@angular/core';
import * as _ from 'lodash';
import { FujiPlayerDetailsService } from '../../services/fujiflush/playerdetails'
import { PlayerDetails, UpdatePlayerDetailsRequest } from '../../models/player'
import { IPlayerSessionComponent } from '../../models/session'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { FujiLocalStorage } from '../../services/fujiflush/localstorage';

@Component({
  selector: 'app-fuji-newplayerdetails-component',
  templateUrl: './newplayerdetails.component.html'
})

export class FujiNewPlayerDetailsComponent implements IPlayerSessionComponent {

  private _playerDetailsService: FujiPlayerDetailsService;
  private _fujiLocalStorage: FujiLocalStorage;
  private _router: Router;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;
  Loading: boolean = true;

  constructor(playerDetailsService: FujiPlayerDetailsService, fujiLocalStorage: FujiLocalStorage,
    router: Router, activatedRoute: ActivatedRoute) {
    this._playerDetailsService = playerDetailsService;    
    this._fujiLocalStorage = fujiLocalStorage;
    this._router = router;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    //Probably replace this with some other kind of validation?
    //this._playerStatusService.Validate(this, PlayerStatus.New, () => { })
    //  .then(data => {
    //    if (data.success) {
    //      this.Loading = false;
    //    }
    //  });
          this.Loading = false;
  }

  public SubmitPlayerDetails(playerDetails: PlayerDetails) {

    var playerDetailsRequest = <UpdatePlayerDetailsRequest>playerDetails;
    playerDetailsRequest.sessionId = this.SessionId;
    playerDetailsRequest.playerId = this.PlayerId;

    this._playerDetailsService.UpdatePlayerDetails(playerDetailsRequest)
      .then(data => {
        if (data.success) {

          this._fujiLocalStorage.CachePlayerDetails(this);
          this._router.navigate(['/fujiflush/sessionlobby', { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
        }
        else {
          this.ErrorMessage = data.errorCode;
        }
      })
      .catch(() => this.HandleGenericError());
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
