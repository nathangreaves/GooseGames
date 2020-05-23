import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsPlayerStatus, WerewordsComponentBase } from '../../../models/werewords/content';
import { WerewordsRoundService } from '../../../services/werewords/round';
import { GenericResponseBase, GenericResponse } from '../../../models/genericresponse';
import { SecretRole, PlayerRoundInformation, RoundOutcomePlayerInformation } from '../../../models/werewords/playerroundinformation';
import { WerewordsPlayerStatusService } from '../../../services/werewords/playerstatus';
import { RoundOutcomeEnum } from '../../../models/werewords/round';
import { WerewordsSessionService } from '../../../services/werewords/session';

@Component({
  selector: 'app-werewords-day-outcome-component',
  templateUrl: './outcome.html',
  styleUrls: ['../common/werewords.common.css']
})
export class WerewordsDayOutcomeComponent extends WerewordsComponentBase implements OnInit {

  DisableButtons: boolean = false;

  VillagersVotedWrong = RoundOutcomeEnum.VillagersVotedWrong;
  WerewolvesVotedSeer = RoundOutcomeEnum.WerewolvesVotedSeer;
  VillagersVotedWerewolf = RoundOutcomeEnum.VillagersVotedWerewolf;
  WerewolvesVotedWrong = RoundOutcomeEnum.WerewolvesVotedWrong;

  RoundOutcome: RoundOutcomeEnum;
  SecretWord: string;
  WerewolfVictory: boolean;
  VillagerVictory: boolean;
  Werewolves: RoundOutcomePlayerInformation[];
  VotedWerewolves: RoundOutcomePlayerInformation[];
  Seer: RoundOutcomePlayerInformation;

  constructor(private roundService: WerewordsRoundService, private playerStatusService: WerewordsPlayerStatusService, private sessionService: WerewordsSessionService) {
    super();
  }

  ngOnInit(): void {

    this.load()
      .then(() => {
        this.Loading = false;
      });
  }

  load(): Promise<any> {
    return this.roundService.GetOutcome(this).then(response => this.HandleGenericResponse(response, data => {
      
      this.SecretWord = data.secretWord;
      this.RoundOutcome = data.roundOutcome;

      this.WerewolfVictory = this.RoundOutcome === this.VillagersVotedWrong || this.RoundOutcome === this.WerewolvesVotedSeer;
      this.VillagerVictory = this.RoundOutcome === this.VillagersVotedWerewolf || this.RoundOutcome === this.WerewolvesVotedWrong;
      this.Werewolves = _.filter(data.players, p => p.secretRole === SecretRole.Werewolf);
      this.VotedWerewolves = _.filter(this.Werewolves, p => p.wasVoted);
      this.Seer = _.find(data.players, p => p.secretRole === SecretRole.Seer);

      return Promise.resolve(<GenericResponseBase>{ success: true })
    }));

  }

  Again() {
    this.DisableButtons = true;

    this.sessionService.Again(this).then(response => this.HandleGenericResponseBase(response, () => {

      this.SetSessionData(this.SessionId, this.PlayerId);
      this.RouteToValidated(WerewordsPlayerStatus.InLobby);

      return Promise.resolve(response);
    }));
  }
}
