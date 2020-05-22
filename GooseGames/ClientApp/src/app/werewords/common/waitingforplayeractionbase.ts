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

  //abstract OnCloseHubConnection(connection: signalR.HubConnection);
  //abstract SetupHubConnection(connection: signalR.HubConnection);

  //constructor(activatedRoute: ActivatedRoute) {
  //  this.SessionId = activatedRoute.snapshot.params.SessionId;
  //  this.PlayerId = activatedRoute.snapshot.params.PlayerId;
  //}

  //OnRedirect() {

  //}

  //CreateHubConnection(): signalR.HubConnectionBuilder {
  //  var hubConnection = new signalR.HubConnectionBuilder()
  //    .withUrl(`/lobbyhub?sessionId=${this.SessionId}&playerId=${this.PlayerId}`);

  //  return hubConnection;
  //}

  //HubConnectionFailed() {
  //  this.HandleGenericError();
  //}

}
