import { WerewordsComponentBase } from "../../../models/werewords/content";
import { GenericResponse } from "../../../models/genericresponse";
import { PlayerAction } from "../../../models/player";
import { WerewordsWaitingForPlayerActionComponent } from "./waitingforplayeraction";

export abstract class WerewordsWaitingForPlayerActionComponentBase extends WerewordsComponentBase {

  PlayerWaitingComponent: WerewordsWaitingForPlayerActionComponent;

  abstract LoadPlayers(): Promise<GenericResponse<PlayerAction[]>>;
  abstract LoadContent(): Promise<any>;
  SetPlayerWaitingComponent(playerWaitingComponent: WerewordsWaitingForPlayerActionComponent) {
    this.PlayerWaitingComponent = playerWaitingComponent;
  }
}
