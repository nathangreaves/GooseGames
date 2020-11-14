import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { LetterJamComponentBase, LetterJamPlayerStatus } from '../../../models/letterjam/content';
import { LetterJamSessionService } from '../../../services/letterjam/session';
import { ILobbyComponentParameters } from '../../components/lobby/lobby';

const MinPlayers: number = 2;
const MaxPlayers: number = 6;

const MaximumNumberOfLetters = 7;
const MinimumNumberOfLetters = 3;
const DefaultNumberOfLetters = 5;

@Component({
  selector: 'letterjam-lobby-component',
  templateUrl: './lobby.component.html',
})
export class LetterJamLobbyComponent extends LetterJamComponentBase implements OnInit, OnDestroy {

  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;
  public NumberOfLetters: number = DefaultNumberOfLetters;

  lobbyParameters: ILobbyComponentParameters;

  DisableButtons: boolean;
  StatusText: string;

  constructor(private _sessionService: LetterJamSessionService, private _router: Router, activatedRoute: ActivatedRoute) {
    super();

    this.Loading = true;

  }

  canStartSession = () => {
    return true;
  }

  startSession = () => {
    return this._sessionService.StartSession(this, this.NumberOfLetters);
  }

  ngOnInit(): void {

    this.HubConnection.on("startingSession", () => {
      this.StatusText = "Setting up the game...";
    });
    this.HubConnection.on("beginSession", (gameId) => {
      this.SetGameId(gameId);
      this.Route(LetterJamPlayerStatus.ConstructingWord);
    });

    this.lobbyParameters = {
      canStartSession: () => true,
      minPlayers: MinPlayers,
      maxPlayers: MaxPlayers,
      playerId: this.PlayerId,
      sessionId: this.SessionId,
      startSession: this.startSession
    }
  }

  ngOnDestroy(): void {
    var connection = this.HubConnection;
    if (connection) {
      connection.off("startingSession");
      connection.off("beginSession");
    }
  }
}
