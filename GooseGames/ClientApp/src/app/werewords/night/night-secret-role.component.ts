import { Component, OnInit, OnDestroy } from '@angular/core';
import * as _ from 'lodash';
import { WerewordsPlayerStatus, WerewordsComponentBase } from '../../../models/werewords/content';
import { WerewordsPlayerRoundInformationService } from '../../../services/werewords/playerroundinformation';
import { GenericResponseBase } from '../../../models/genericresponse';
import { SecretRole, OtherPlayerSecretRoleResponse } from '../../../models/werewords/playerroundinformation';
import { WerewordsPlayerStatusService } from '../../../services/werewords/playerstatus';

@Component({
  selector: 'app-werewords-night-secret-role-component',
  templateUrl: './night-secret-role.component.html'
})
export class WerewordsNightSecretRoleComponent extends WerewordsComponentBase implements OnInit, OnDestroy {
  SecretRole: SecretRole;
  KnowledgeAboutOtherPlayers: OtherPlayerSecretRoleResponse[];
  MayorName: string;
  MayorId: string;
  IsMayor: boolean;
  ButtonsEnabled: boolean = true;
  WaitingForMayor: boolean = false;

  constructor(private roundInformationService: WerewordsPlayerRoundInformationService, private playerStatusService: WerewordsPlayerStatusService) {
    super();
  }

  ngOnInit() {
    this.load()
      .then(() => {

        if (this.CurrentStatus == WerewordsPlayerStatus.NightWaitingForMayor) {
          this.WaitingForMayor = true;
        }
        this.Loading = false;

      });

    this.HubConnection.on("secretWord", () => {
      this.Route(WerewordsPlayerStatus.NightSecretWord);
    });
  }

  ngOnDestroy(): void {
    this.HubConnection.off("secretWord");
  }

  //TODO: Setup Hub connection to listen for secret word.

  load(): Promise<any> {
    return this.roundInformationService.GetSecretRole(this)
      .then(response => {
        return this.HandleGenericResponse(response, r => {

          this.SecretRole = r.secretRole;
          this.MayorName = r.mayorName;
          this.MayorId = r.mayorPlayerId;
          this.IsMayor = r.mayorPlayerId.toLowerCase() == this.PlayerId.toLowerCase();
          this.KnowledgeAboutOtherPlayers = r.knowledgeAboutOtherPlayers;

          return Promise.resolve(<GenericResponseBase>{ success: true });
        });
      })
      .catch(e => this.HandleGenericError(e));
  }

  Continue() {
    this.ButtonsEnabled = false;

    this.playerStatusService.TransitionToNextNightStatus(this)
      .then(response => {
        return this.HandleGenericResponse(response, data => {

          if (data === WerewordsPlayerStatus.NightWaitingForMayor) {
            this.CurrentStatus = data;
            this.WaitingForMayor = true;
          }
          else {
            this.RouteToValidated(data);
          }

          return Promise.resolve(<GenericResponseBase>{ success: true });
        });
      })
      .catch(e => this.HandleGenericError(e));
    //TODO: If mayor, move to pick word
    //      If not mayor, move to waiting for mayor
    //      Not everyone needs to be on the same step during the night phase, the only blocker is the Mayor if they haven't picked their word yet.
  }

  //ChangePage() {
  //  this.Route(WerewordsPlayerStatus.NightSecretWord);
  //}
}
