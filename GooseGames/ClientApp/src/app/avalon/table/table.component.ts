import { Component, OnInit, OnDestroy } from '@angular/core';
import { AvalonComponentBase, AllPlayersFromCacheRequest } from '../../../models/avalon/content';
import { AvalonRolesService } from '../../../services/avalon/roles';
import { AvalonRole, GetAvalonRoleDetail, AvalonRoleEnum } from '../../../models/avalon/roles';
import { AvalonPlayerService } from '../../../services/avalon/player';
import { AvalonPlayer, AvalonPlayerIntel } from '../../../models/avalon/player';
import _ from 'lodash';

@Component({
  selector: 'avalon-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.scss']
})
export class AvalonTableComponent extends AvalonComponentBase implements OnInit, OnDestroy {

  RolesInPlay: AvalonRole[];
  Players: AvalonPlayer[];
  ShowSecretInfo: boolean = false;

  constructor(private _roleService: AvalonRolesService, private _playerService: AvalonPlayerService)
  {
    super();
    this.Loading = true;
  }

  OnShowSecretInfoClick = () => {
    this.ShowSecretInfo = !this.ShowSecretInfo;
  }

  ngOnInit(): void {

    this._roleService.GetCurrentGameRoles(this)
      .then(response => this.HandleGenericResponse(response, r => {

        this.RolesInPlay = r.map(role => {
          return <AvalonRole>{
            ...role,
            ...GetAvalonRoleDetail(role.roleEnum),
            selected: true
          }
        });

        return response;
      }))
      .then(() => this._playerService.GetPlayers(this))
      .then(response => this.HandleGenericResponse(response, r => {

        this.Players = r.map(player => {
          return <AvalonPlayer>{
            ...player,
            actualRole: <AvalonRole>{
              ...player.actualRole,
              ...GetAvalonRoleDetail(player.actualRole.roleEnum),
              selected: true
            },
            assumedRole: <AvalonRole>{
              ...player.assumedRole,
              ...GetAvalonRoleDetail(player.assumedRole.roleEnum),
              selected: true
            },
            actualAndAssumedDifferent: player.actualRole.roleEnum !== player.assumedRole.roleEnum,
            playerIntel: player.playerIntel.map(pI => {
              return <AvalonPlayerIntel>{
                ...pI,
                roleName: pI.roleKnowsYou !== null ? GetAvalonRoleDetail(pI.roleKnowsYou).name : null,
                intelPlayer: null
              }
            }),
            player: null
          }
        });
        return response;
      }))
      .then(() => this.GetPlayersFromCache(new AllPlayersFromCacheRequest()))
      .then(p => {
        _.each(this.Players, player => {
          player.player = _.find(p, pl => pl.id == player.playerId);
          _.each(player.playerIntel, pI => {
            (<AvalonPlayerIntel>pI).intelPlayer = _.find(p, pl => pl.id == pI.intelPlayerId);
          })
        })
      })
      .finally(() => {
        this.Loading = false;
      });
  }

  ngOnDestroy(): void {

  }

  showDrunkViability = () => {
    return _.find(this.RolesInPlay, r => r.roleEnum === AvalonRoleEnum.Drunk);
  }
  showMyopiaViability = () => {
    return _.find(this.RolesInPlay, r => r.roleEnum === AvalonRoleEnum.Myopia);
  }


}
