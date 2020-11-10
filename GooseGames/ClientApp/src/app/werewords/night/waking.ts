import { Component, OnInit, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsPlayerStatus, WerewordsComponentBase } from '../../../models/werewords/content';
import { WerewordsRoundService } from '../../../services/werewords/round';
import { GenericResponseBase, GenericResponse } from '../../../models/genericresponse';
import { SecretRole } from '../../../models/werewords/playerroundinformation';
import { WerewordsPlayerStatusService } from '../../../services/werewords/playerstatus';
import { WerewordsWaitingForPlayerActionComponentBase } from '../common/waitingforplayeractionbase';
import { PlayerAction } from '../../../models/player';
import { WerewordsWaitingForPlayerActionComponent } from '../common/waitingforplayeraction';

@Component({
  selector: 'app-werewords-night-waking-component',
  templateUrl: './waking.html'
})
export class WerewordsNightWakingComponent extends WerewordsWaitingForPlayerActionComponentBase implements OnInit, OnDestroy {
    
  _playerWaitingComponent: WerewordsWaitingForPlayerActionComponent;

  constructor(private playerStatusService: WerewordsPlayerStatusService) {
    super();
  }

  ngOnInit(): void {

    this.HubConnection.on("playerAwake", (playerAction: PlayerAction) => {
      this.PlayerWaitingComponent.HandlePlayerAction(playerAction);
    });

    this.HubConnection.on("dayBegin", () => {
      this.Route(WerewordsPlayerStatus.DayPassive);
    });

  }

  ngOnDestroy(): void {
    this.HubConnection.off("playerAwake");
    this.HubConnection.off("dayBegin");
  }

  LoadPlayers(): Promise<GenericResponse<PlayerAction[]>> {
    return this.playerStatusService.GetAwakePlayers(this);
  }
  LoadContent(): Promise<any> {
    return Promise.resolve();
  }

}
