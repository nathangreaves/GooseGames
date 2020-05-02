import { IPlayerSession } from "../../models/session";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root',
})
export class JustOneLocalStorage {

  CachePlayerDetails(playerSession: IPlayerSession) {
    localStorage.setItem("just-one-session-id", playerSession.SessionId);
    localStorage.setItem("just-one-player-id", playerSession.PlayerId);
    const numberOfMillisecondsPerHour = 3600000;
    localStorage.setItem("just-one-expiry", (new Date().getTime() + numberOfMillisecondsPerHour).toString());
  }

  GetPlayerDetails(): IPlayerSession {
    var expiry = localStorage.getItem("just-one-expiry");

    if (expiry && Number(expiry) > new Date().getTime()) {

      var sessionId = localStorage.getItem("just-one-session-id");
      var playerId = localStorage.getItem("just-one-player-id");

      if (sessionId && playerId) {
        return {
          SessionId: sessionId,
          PlayerId: playerId
        }
      }

      return null;
    }
  }

  ClearPlayerDetails() {
    localStorage.removeItem("just-one-session-id");
    localStorage.removeItem("just-one-player-id");
    localStorage.removeItem("just-one-expiry");
  }
}
