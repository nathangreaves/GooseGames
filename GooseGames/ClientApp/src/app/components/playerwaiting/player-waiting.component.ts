import * as _ from 'lodash';
import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IGooseGamesPlayer, GooseGamesPlayerAction, IGooseGamesPlayerAction } from '../../../models/player';

export interface IWaitingForPlayerActionParameters {
  loadPlayers: () => Promise<IGooseGamesPlayer[]>;
  loadPlayerActions: () => Promise<IGooseGamesPlayerAction[]>;
  hubConnection: signalR.HubConnection;
  hubConnectionString: string;
}

@Component({
  selector: 'gg-player-waiting',
  templateUrl: './player-waiting.component.html',
  styleUrls: ['./player-waiting.component.scss']
})
export class PlayerWaitingComponent implements OnInit, OnDestroy {

  Players: GooseGamesPlayerAction[] = [];
  @Input() parameters: IWaitingForPlayerActionParameters;  

  ngOnInit() {
    Promise.all([this.parameters.loadPlayers(), this.parameters.loadPlayerActions()])
      .then(responses => {

        _.each(responses[0], player => {
          var playerActionResponse = _.find(responses[1], pA => pA.playerId === player.id);

          var playerAction = {
            ...player,
            hasTakenAction: playerActionResponse && playerActionResponse.hasTakenAction
          }

          this.Players.push(playerAction);
        });

        this.parameters.hubConnection.on(this.parameters.hubConnectionString, this.PlayerHasTakenAction);
      });
  }

  ngOnDestroy() {
    this.parameters.hubConnection.off(this.parameters.hubConnectionString, this.PlayerHasTakenAction);
  }

  PlayerHasTakenAction = (playerId: string, hasTakenAction: boolean) => {
    if (playerId) {
      _.find(this.Players, p => {
        return p.id == playerId;
      }).hasTakenAction = hasTakenAction;
    }
  }
}
