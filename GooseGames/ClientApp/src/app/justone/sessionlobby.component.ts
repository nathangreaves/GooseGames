import { Component, OnInit, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { JustOneSessionService } from '../../services/justone/session'
import { GenericResponseBase } from '../../models/genericresponse'
import { PlayerDetailsResponse } from '../../models/player'
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as signalR from "@microsoft/signalr";
import { JustOnePlayerStatusService } from '../../services/justone/playerstatus'
import { PlayerStatus, PlayerStatusRoutesMap } from '../../models/justone/playerstatus'
import { IPlayerSessionComponent } from '../../models/session';
import { PlayerNumberCss } from '../../services/justone/ui'
import { WordListCheckboxListItem, JustOneWordList } from '../../models/justone/wordlistenum';
import { ILobbyComponentParameters } from '../components/lobby/lobby';
import { NavbarService } from '../../services/navbar';

const MinPlayers: number = 3;
const MaxPlayers: number = 7;

@Component({
  selector: 'app-just-one-sessionlobby-component',
  templateUrl: './sessionlobby.component.html',
  styleUrls: ['./sessionlobby.component.css'],
})
export class JustOneSessionLobbyComponent implements IPlayerSessionComponent, OnInit, OnDestroy {

  PlayerNumberCSS = PlayerNumberCss;

  private _sessionService: JustOneSessionService;
  private _playerStatusService: JustOnePlayerStatusService;
  private _router: Router;
  private _hubConnection: signalR.HubConnection;

  SessionId: string;
  PlayerId: string;
  ErrorMessage: string;

  public Loading: boolean;
  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;
  public Players: PlayerDetailsResponse[];
  public AvailableWordLists: WordListCheckboxListItem[];

  DisableButtons: boolean;
  lobbyParameters: ILobbyComponentParameters;

  constructor(sessionService: JustOneSessionService,
    playerStatusService: JustOnePlayerStatusService,
    private navbarService: NavbarService,
              router: Router,
    activatedRoute: ActivatedRoute) {
    this._playerStatusService = playerStatusService;
    this._sessionService = sessionService;
    this._router = router;

    this.Loading = true;

    this.SessionId = activatedRoute.snapshot.params.SessionId;
    this.PlayerId = activatedRoute.snapshot.params.PlayerId;

    this.setupConnection()
      .then(this.Validate);
  }

  ngOnInit(): void {

    this.navbarService.reset();
    this.navbarService.setAreaTitle("Just One");

    this.lobbyParameters = {
      canStartSession: this.canStartSession,
      maxPlayers: MaxPlayers,
      minPlayers: MinPlayers,
      playerId: this.PlayerId,
      sessionId: this.SessionId,
      startSession: this.startGame,
      startingSessionMessage: "Starting game. JUST ONE moment please.",
      playerIsSessionMaster: (isSessionMaster: boolean) => { }
    }

    this.AvailableWordLists = [
      <WordListCheckboxListItem>{
        Name: "Just One",
        Checked: true,
        WordList: JustOneWordList.JustOne
      },
      <WordListCheckboxListItem>{
        Name: "Codenames",
        Checked: true,
        WordList: JustOneWordList.Codenames
      },
      <WordListCheckboxListItem>{
        Name: "Codenames Duet",
        Checked: true,
        WordList: JustOneWordList.CodenamesDuet
      },
      <WordListCheckboxListItem>{
        Name: "Codenames Rude Words 😲",
        Checked: false,
        WordList: JustOneWordList.CodenamesDeepUndercover
      }
    ];
  }

  ngOnDestroy() {
    this.CloseConnection();
  }

  canStartSession = () : boolean => {
    if (!_.find(this.AvailableWordLists, p => p.Checked)) {
      this.ErrorMessage = "Please select at least one word list";
      return false;
    }

    return true;
  }

  private Validate = (): Promise<any> => {
    return this._playerStatusService.Validate(this, PlayerStatus.InLobby, () => { this.CloseConnection(); }).finally(() => this.Loading = false);
  }   

  startGame = (): Promise<GenericResponseBase> => {
    return this._sessionService
      .StartSession(this, _.filter(this.AvailableWordLists, w => w.Checked).map(w => w.WordList));      
  }

  private setupConnection(): Promise<any> {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`)
      .withAutomaticReconnect()
      .build();

    this._hubConnection.onreconnected(() => {
      this.Validate();
    });
    this._hubConnection.onclose(() => {
      this.HandleGenericError();
    });

    this._hubConnection.on("beginRoundPassivePlayer", () => {
      this.CloseConnection();
      this._router.navigate([PlayerStatusRoutesMap.PassivePlayerClue, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    this._hubConnection.on("beginRoundActivePlayer", () => {
      this.CloseConnection();
      this._router.navigate([PlayerStatusRoutesMap.ActivePlayerWaitingForClues, { SessionId: this.SessionId, PlayerId: this.PlayerId }]);
    });
    return this._hubConnection.start().catch(err => console.error(err));
  }

  CloseConnection() {
    var connection = this._hubConnection;
    if (connection) {
      this._hubConnection = null;
      connection.off("beginRoundPassivePlayer");
      connection.off("beginRoundActivePlayer");

      connection.onclose(() => { });

      connection.stop();
    }
  }

  HandleGenericError() {
    this.ErrorMessage = "An Unknown Error Occurred";
  }
}
