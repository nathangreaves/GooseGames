import { Component, OnInit, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsPlayerStatus, WerewordsComponentBase } from '../../../models/werewords/content';
import { WerewordsRoundService } from '../../../services/werewords/round';
import { GenericResponseBase, GenericResponse } from '../../../models/genericresponse';
import { WerewordsPlayerStatusService } from '../../../services/werewords/playerstatus';
import { PlayerRoundInformation, PlayerResponseType, PlayerResponse } from '../../../models/werewords/playerroundinformation';

@Component({
  selector: 'app-werewords-day-component',
  templateUrl: './day.html',
  styleUrls: ['./day.css']
})
export class WerewordsDayComponent extends WerewordsComponentBase implements OnInit, OnDestroy {
  MayorName: string;
  MayorId: string;
  IsMayor: boolean;
  DisableButtons: boolean = false;
  Players: PlayerRoundInformation[];
  IsActive: boolean;

  TickResponseType = PlayerResponseType.Tick;
  CrossResponseType = PlayerResponseType.Cross;
  QuestionMarkResponseType = PlayerResponseType.QuestionMark;
  SoCloseResponseType = PlayerResponseType.SoClose;
  WayOffResponseType = PlayerResponseType.WayOff;
  CorrectResponseType = PlayerResponseType.Correct;
  TimeMinutes: string;
  TimeSeconds: string;
  RoundStarted: boolean;
  Voting: boolean;

  constructor(private roundService: WerewordsRoundService, private playerStatusService: WerewordsPlayerStatusService) {
    super();
  }
  ngOnInit(): void {

    this.HubConnection.on("playerResponse", (playerResponse: PlayerResponse) => {

      var player = _.find(this.Players, p => p.id.toLowerCase() == playerResponse.playerId.toLowerCase());
      player.responses.push(playerResponse);

    });
    this.HubConnection.on("activePlayer", (playerId: string) => {
      _.each(this.Players, p => {
        p.active = false;
      });
      var player = _.find(this.Players, p => p.id.toLowerCase() == playerId.toLowerCase());
      player.active = true;
      this.DisableButtons = false;

      this.IsActive = player.id.toLowerCase() == this.PlayerId;
    });
    this.HubConnection.on("voteWerewolves", (endTime: string) => {
      this.Voting = true;
      this.RoundStarted = false;
      this.startTimer(new Date(endTime));
    });
    this.HubConnection.on("voteSeer", (endTime: string) => {
      this.Voting = true;
      this.RoundStarted = false;
      this.startTimer(new Date(endTime));
    });
    this.HubConnection.on("startTimer", (endTime: string) => {
      this.RoundStarted = true;
      this.startTimer(new Date(endTime));
    });

    this.load()
      .then(() => {
        this.Loading = false;
      });
  }
  ngOnDestroy(): void {
    this.HubConnection.off("playerResponse");
    this.HubConnection.off("activePlayer");
    this.HubConnection.off("voteWerewolves");
    this.HubConnection.off("voteSeer");
    this.HubConnection.off("startTimer");
  }


  startTimer(endTime: Date) {

    var endTimeAsMilliseconds = endTime.getTime();

    var timer = setInterval(() => {

      Date.now();

      var millis = endTimeAsMilliseconds - new Date().getTime();

      if (millis > 0) {
        var minutes = Math.floor(millis / 60000);
        var seconds = Math.floor(((millis % 60000) / 1000));

        if (seconds == 60) {
          minutes = minutes + 1;
          seconds = 0;
        }
        this.TimeMinutes = minutes.toString();
        this.TimeSeconds = (seconds < 10 ? "0" : "") + seconds.toFixed(0);
      }
      else {
        this.TimeMinutes = "0";
        this.TimeSeconds = "00";
        clearInterval(timer);
      }
    }, 500);
  }

  load(): Promise<any> {
    return this.roundService.GetDay(this).then(response => this.HandleGenericResponse(response, data => {

      this.MayorName = data.mayorName;
      this.MayorId = data.mayorPlayerId;
      this.IsMayor = data.mayorPlayerId.toLowerCase() == this.PlayerId.toLowerCase();
      this.Players = data.players;
      this.IsActive = data.isActive;

      if (this.CurrentStatus == WerewordsPlayerStatus.DayVotingOnSeer) {
        this.Voting = true;
      }
      if (this.CurrentStatus == WerewordsPlayerStatus.DayVotingOnWerewolves) {
        this.Voting = true;
      }

      if (data.voteEndTime) {
        this.startTimer(new Date(data.voteEndTime));
      }
      if (data.endTime) {
        this.startTimer(new Date(data.endTime));
      }

      return Promise.resolve(<GenericResponseBase>{ success: true })
    }));
  }


  Tick(player: PlayerRoundInformation) {
    this.SendResponse(player, PlayerResponseType.Tick);
  }
  Cross(player: PlayerRoundInformation) {
    this.SendResponse(player, PlayerResponseType.Cross);
  }
  Question(player: PlayerRoundInformation) {
    this.SendResponse(player, PlayerResponseType.QuestionMark);
  }
  SoClose(player: PlayerRoundInformation) {
    this.SendResponse(player, PlayerResponseType.SoClose);
  }
  WayOff(player: PlayerRoundInformation) {
    this.SendResponse(player, PlayerResponseType.WayOff);
  }
  Correct(player: PlayerRoundInformation) {
    this.SendResponse(player, PlayerResponseType.Correct);
  }
  Start() {
    this.roundService.Start(this);
  }

  SendResponse(player: PlayerRoundInformation, responseType: PlayerResponseType) {
    this.DisableButtons = true;

    this.roundService.PlayerResponse(this, player, responseType)
      .then(response => this.HandleGenericResponseBase(response, () => Promise.resolve(response)))
      .finally(() => {
        //this.DisableButtons = false;
      });
  }

}
