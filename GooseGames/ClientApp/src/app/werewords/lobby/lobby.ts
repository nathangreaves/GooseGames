import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { WerewordsComponentBase, WerewordsPlayerStatus } from '../../../models/werewords/content';
import { WerewordsSessionService } from '../../../services/werewords/session';
import { ILobbyComponentParameters } from '../../components/lobby/lobby';

const MinPlayers: number = 4;
const MaxPlayers: number = 10;

@Component({
  selector: 'app-werewords-lobby-component',
  templateUrl: './lobby.html',
})
export class WerewordsLobbyComponent extends WerewordsComponentBase implements OnInit, OnDestroy {

  public SessionMaster: boolean;
  public SessionMasterName: string;
  public SessionMasterPlayerNumber: number;
  public Password: string;

  lobbyParameters: ILobbyComponentParameters;

  DisableButtons: boolean;

  constructor(private _sessionService: WerewordsSessionService, private _router: Router, activatedRoute: ActivatedRoute) {
    super();

    this.Loading = true;

  }

  canStartSession = () => {
    return true;
  }

  startSession = () => {
    return this._sessionService.StartSession(this);
  }

  ngOnInit(): void {

    this.HubConnection.on("secretRole", () => {      
      this.Route(WerewordsPlayerStatus.NightRevealSecretRole);
    });

    this.lobbyParameters = {
      canStartSession: () => true,
      minPlayers: MinPlayers,
      maxPlayers: MaxPlayers,
      playerId: this.PlayerId,
      sessionId: this.SessionId,
      startSession: this.startSession,
      startingSessionMessage: "Starting session. WEREWORDS moment please."
    }
  }

  ngOnDestroy(): void {
    var connection = this.HubConnection;
    if (connection) {
      connection.off("secretRole");
    }
  }
}
