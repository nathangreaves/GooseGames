import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AvalonComponentBase, AvalonPlayerStatus } from '../../../models/avalon/content';
import { AvalonSessionService } from '../../../services/avalon/session';
import { ILobbyComponentParameters } from '../../components/lobby/lobby';
import { AvalonRoleEnum, IAvalonRole, AvalonRole, GetAvalonRoleDetail } from '../../../models/avalon/roles';
import { AvalonRolesService } from '../../../services/avalon/roles';
import _ from 'lodash';
import { PlayerDetailsResponse } from '../../../models/player';

const MinPlayers: number = 6;
const MaxPlayers: number = 13;

@Component({
  selector: 'avalon-lobby-component',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss'],
})
export class AvalonLobbyComponent extends AvalonComponentBase implements OnInit, OnDestroy {

  public SessionMaster: boolean;
  public SelectedRoles: AvalonRoleEnum[] = [];
  public AllRoles: AvalonRole[];
  public Weight: number = 0;
  public ShowDescription: boolean = false;
  public OnlyShowSelected: boolean = true;

  lobbyParameters: ILobbyComponentParameters;

  DisableButtons: boolean;
  StatusText: string;

  constructor(private _sessionService: AvalonSessionService, private _avalonRolesService: AvalonRolesService, private _router: Router, activatedRoute: ActivatedRoute) {
    super();

    this.Loading = true;
  }

  canStartSession = () => {
    return true;
  }

  startSession = () => {
    return this._sessionService.StartSession(this, this.SelectedRoles);
  }

  ngOnInit(): void {
    this.HubConnection.on("beginSession", (gameId) => {
      this.SetGameId(gameId);
      this.Route(AvalonPlayerStatus.InGame);
    });

    this.HubConnection.on("selectedRoles", (playerId: string, roles: AvalonRoleEnum[]) => {
      if (true) {
        if (this.PlayerId !== playerId) {
          this.SelectedRoles = roles;
          _.each(this.AllRoles, allRole => {
            var role = _.find(roles, selectedRole => allRole.roleEnum === selectedRole);
            allRole.selected = role !== null && role !== undefined;
          });
        }
      }
    });

    this.HubConnection.on("requestedSelectedRoles", () => {
      if (this.SessionMaster) {
        this.pushSelectedRoles();
      }
    });
    this.HubConnection.on("requestedSelectedRolesAsSessionMaster", () => {
      if (!this.SessionMaster) {
        this.pushSelectedRoles();
      }
    });
    this.HubConnection.on("weight", (weight: number) => {
      if (!this.SessionMaster) {
        this.Weight = weight;
      }
    });

    this.lobbyParameters = {
      canStartSession: () => true,
      minPlayers: MinPlayers,
      maxPlayers: MaxPlayers,
      playerId: this.PlayerId,
      sessionId: this.SessionId,
      startSession: this.startSession,
      startingSessionMessage: "Starting game. AVALON moment please.",
      playerIsSessionMaster: (isSessionMaster: boolean) => {
        this.SessionMaster = isSessionMaster;
        if (this.SessionMaster) {
          this.HubConnection.send("requestSelectedRolesAsSessionMaster", this.SessionId);
          this.OnlyShowSelected = false;
        }
        else {
          this.HubConnection.send("requestSelectedRoles", this.SessionId);
        }
      }
    }

    this.load();
  }

  load = () => {
    this._avalonRolesService
      .GetAllRoles(this)
      .then(response => this.HandleGenericResponse(response, r => {
        this.AllRoles = r.map(role => {
          return <AvalonRole>
            {
              ...role,
              ...GetAvalonRoleDetail(role.roleEnum),
              selected: false
            }
        });

        return response;
      }))
      .finally(() => {
        this.Loading = false;
      });
  }

  onRoleClicked = (role: AvalonRole) => {
    if (role.selected) {
      var index = _.findIndex(this.SelectedRoles, r => r === role.roleEnum);
      if (index >= 0) {
        this.SelectedRoles.splice(index, 1);
      }
    }
    else {
      this.SelectedRoles.push(role.roleEnum);
    }
    role.selected = !role.selected;
    this.pushSelectedRoles();
    this._avalonRolesService.GetWeight(this, this.SelectedRoles)
      .then(response => this.HandleGenericResponse(response, r => {

        this.Weight = r;

        return response;
      }))
  }

  private pushSelectedRoles() {
    this.HubConnection.invoke("pushSelectedRoles", this.PlayerId, this.SessionId, this.SelectedRoles);    
  }

  showDrunkViability = () => {
    return _.find(this.SelectedRoles, r => r === AvalonRoleEnum.Drunk);
  }
  showMyopiaViability = () => {
    return _.find(this.SelectedRoles, r => r === AvalonRoleEnum.Myopia);
  }

  goodWidth = () => {
    return (50 + (this.Weight * 5)) + "%" ;
  }
  evilWidth = () => {
    return (50 - (this.Weight * 5)) + "%";
  }

  ngOnDestroy(): void {
    var connection = this.HubConnection;
    if (connection) {
      connection.off("beginSession");
      connection.off("selectedRoles");
      connection.off("requestedSelectedRoles");
      connection.off("requestedSelectedRolesAsSessionMaster");
    }
  }
}
