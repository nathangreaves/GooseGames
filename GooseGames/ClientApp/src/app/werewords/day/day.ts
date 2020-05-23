import { Component, OnInit, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsPlayerStatus, WerewordsComponentBase } from '../../../models/werewords/content';
import { WerewordsRoundService } from '../../../services/werewords/round';
import { GenericResponseBase, GenericResponse } from '../../../models/genericresponse';
import { WerewordsPlayerStatusService } from '../../../services/werewords/playerstatus';
import { PlayerRoundInformation, PlayerResponseType, PlayerResponse, SecretRole } from '../../../models/werewords/playerroundinformation';

@Component({
  selector: 'app-werewords-day-component',
  templateUrl: './day.html',
  styleUrls: ['./day.css', '../common/werewords.common.css']
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

  DayMayorStatus = WerewordsPlayerStatus.DayMayor;
  VotingOnSeerStatus = WerewordsPlayerStatus.DayVotingOnSeer;
  VotingOnWerewolfStatus = WerewordsPlayerStatus.DayVotingOnWerewolves;

  WerewolfSecretRole = SecretRole.Werewolf;

  //NumberOfWerewolves = () => { return this.Werewolves().length }

  Werewolves: PlayerRoundInformation[];
  IsWerewolf: boolean;

  TimeMinutes: string;
  TimeSeconds: string;
  RoundStarted: boolean;
  Voting: boolean;
  _timer: NodeJS.Timeout;
  SecretWord: string;
  ConfirmSoClose: boolean;
  ConfirmWayOff: boolean;
  ConfirmCorrect: boolean;
  SoCloseSpent: boolean;
  WayOffSpent: boolean;

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
    this.HubConnection.on("voteWerewolves", (endTime: string, secretWord: string) => {
      this.Voting = true;
      this.SecretWord = secretWord;
      this.startTimer(new Date(endTime));
      this.revealMayor();
      this.CurrentStatus = WerewordsPlayerStatus.DayVotingOnWerewolves;
    });
    this.HubConnection.on("voteSeer", (endTime: string, werewolves: string[], secretWord: string) => {
      this.Voting = true;
      this.SecretWord = secretWord;
      this.startTimer(new Date(endTime));

      _.each(werewolves, w => {
        var player = _.find(this.Players, p => p.id.toLowerCase() == w.toLowerCase());
        player.secretRole = SecretRole.Werewolf;
      });
      this.revealMayor();
      this.setWerewolves();
      this.CurrentStatus = WerewordsPlayerStatus.DayVotingOnSeer;
    });
    this.HubConnection.on("startTimer", (endTime: string) => {
      this.RoundStarted = true;
      this.startTimer(new Date(endTime));
    });
    this.HubConnection.on("roundOutcome", () => {
      this.Route(WerewordsPlayerStatus.DayOutcome);
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
    this.HubConnection.off("roundOutcome");
  }


  startTimer(endTime: Date) {

    var endTimeAsMilliseconds = endTime.getTime();

    clearInterval(this._timer);
    this._timer = setInterval(() => {

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

        clearInterval(this._timer);
        this.timerFinished();
      }
    }, 500);
  }
  timerFinished() {
    if (this.CurrentStatus == WerewordsPlayerStatus.DayMayor) {

    }
    else if (this.CurrentStatus == WerewordsPlayerStatus.DayVotingOnSeer) {
      this.Voting = false;
    }
    else if (this.CurrentStatus == WerewordsPlayerStatus.DayVotingOnWerewolves) {
      this.Voting = false;
    }

    if (this.IsMayor) {
      this.load();
    }
  }

  setWerewolves() {
    this.Werewolves = _.filter(this.Players, p => p.secretRole === SecretRole.Werewolf);
    this.IsWerewolf = !!_.find(this.Players, p => p.id.toLowerCase() == this.PlayerId.toLowerCase() && p.secretRole === SecretRole.Werewolf);
  }
  revealMayor() {
    var mayor = _.find(this.Players, p => p.isMayor);
    mayor.isHidden = false;
  }

  hideMayor(players: PlayerRoundInformation[]) {
    var mayor = _.find(players, p => p.isMayor);
    mayor.isHidden = true;
  }

  activePlayer(): PlayerRoundInformation {
    return _.find(this.Players, p => p.active);
  }

  load(): Promise<any> {
    return this.roundService.GetDay(this).then(response => this.HandleGenericResponse(response, data => {

      this.MayorName = data.mayorName;
      this.MayorId = data.mayorPlayerId;
      this.IsMayor = data.mayorPlayerId.toLowerCase() == this.PlayerId.toLowerCase();
      var players = data.players;
      this.IsActive = data.isActive;
      this.SecretWord = data.secretWord
      this.SoCloseSpent = data.soCloseSpent;
      this.WayOffSpent = data.wayOffSpent;

      if (this.CurrentStatus == WerewordsPlayerStatus.DayVotingOnSeer) {
        this.Voting = true;
      }
      else if (this.CurrentStatus == WerewordsPlayerStatus.DayVotingOnWerewolves) {
        this.Voting = true;
      }
      else {
        this.hideMayor(players);
      }
      this.Players = players;
      if (data.voteEndTime) {
        this.startTimer(new Date(data.voteEndTime));
        this.RoundStarted = true;
      }
      else if (data.endTime) {
        this.startTimer(new Date(data.endTime));
        this.RoundStarted = true;
      }
      this.setWerewolves();

      return Promise.resolve(<GenericResponseBase>{ success: true })
    }));
  }

  VotePlayer(player: PlayerRoundInformation) {

    if (!this.CanVote()) {
      return;
    }

    _.each(this.Players, p => { p.isVoted = false; })

    player.isVoted = true;

    if (this.CurrentStatus === this.VotingOnSeerStatus) {
      this.roundService.VoteAsSeer(this, player.id);
    }
    else if (this.CurrentStatus === this.VotingOnWerewolfStatus) {
      this.roundService.VoteAsWerewolf(this, player.id);
    }
  }
  CanVote() {
    return this.Voting && ((this.CurrentStatus === this.VotingOnSeerStatus && this.IsWerewolf) || (this.CurrentStatus === this.VotingOnWerewolfStatus))
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
    this.ConfirmSoClose = true;
  }
  SoCloseConfirmed() {
    this.SendResponse(this.activePlayer(), PlayerResponseType.SoClose);
  }
  WayOff(player: PlayerRoundInformation) {
    this.ConfirmWayOff = true;    
  }
  WayOffConfirmed() {
    this.SendResponse(this.activePlayer(), PlayerResponseType.WayOff);
  }
  Correct(player: PlayerRoundInformation) {
    this.ConfirmCorrect = true; 
  }
  CorrectConfirmed() {
    this.SendResponse(this.activePlayer(), PlayerResponseType.Correct);
  }
  Confirm() {
    if (this.ConfirmSoClose) {
      this.SoCloseConfirmed();
      this.SoCloseSpent = true;
      this.CancelConfirm();
    }
    else if (this.ConfirmWayOff) {
      this.WayOffConfirmed();
      this.WayOffSpent = true;
      this.CancelConfirm();
    }
    else if (this.ConfirmCorrect) {
      this.CorrectConfirmed();
      this.CancelConfirm();
    }
  }
  CancelConfirm() {
    this.ConfirmSoClose = false;
    this.ConfirmWayOff = false;
    this.ConfirmCorrect = false;
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
