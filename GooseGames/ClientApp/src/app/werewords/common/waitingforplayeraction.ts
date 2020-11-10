import * as _ from 'lodash';
import { Component, Input, OnInit } from '@angular/core';
import { PlayerAction } from '../../../models/player';
import { WerewordsWaitingForPlayerActionComponentBase } from './waitingforplayeractionbase';

@Component({
  selector: 'app-werewords-playerwaiting-component',
  templateUrl: './waitingforplayeraction.html',
  styleUrls: ['./waitingforplayeraction.css']
})
export class WerewordsWaitingForPlayerActionComponent implements OnInit {

  Players: PlayerAction[];
  @Input() playerWaitingComponent: WerewordsWaitingForPlayerActionComponentBase;  

  ngOnInit() {
    this.playerWaitingComponent.SetPlayerWaitingComponent(this);

    this.playerWaitingComponent.LoadPlayers()
      .then(response => {
        if (response && response.success) {
          this.Players = response.data;
        }
        else if (response) {
          this.playerWaitingComponent.ErrorMessage = response.errorCode;
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {
          this.playerWaitingComponent.LoadContent();
        }
        return response;
      })
      .then(response => {
        if (response && response.success) {
          this.playerWaitingComponent.Loading = false;
        }
      })
      .catch(err => {
        this.playerWaitingComponent.HandleGenericError(err);
      });
  }

  HandlePlayerAction(playerAction: PlayerAction) {
    if (playerAction && playerAction.hasTakenAction && this.Players && this.Players.length > 0) {
      _.find(this.Players, p => {
        return p.id == playerAction.id;
      }).hasTakenAction = true;
    }
  }
  PlayerHasTakenAction(playerId: string, hasTakenAction: boolean) {
    if (playerId) {
      _.find(this.Players, p => {
        return p.id == playerId;
      }).hasTakenAction = hasTakenAction;
    }
  }
}
