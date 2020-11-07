import { Injectable } from "@angular/core";
import { IPlayerSession } from "../models/session";

@Injectable({
  providedIn: 'root',
})
export class GooseGamesLocalStorage {

  CachePlayerName(name: string) {
    localStorage.setItem("goose-games-player-name", name);
  }

  GetPlayerName(): string {
    return localStorage.getItem("goose-games-player-name");
  }

  //Clear() {
  //  localStorage.removeItem("goose-games-player-name");
  //}

  CachePlayerDetails(playerSession: IPlayerSession, playerName: string = null) {
    localStorage.setItem("goose-games-session-id", playerSession.SessionId);
    localStorage.setItem("goose-games-player-id", playerSession.PlayerId);

    //Do we want this info to expire?
    //const numberOfMillisecondsPerHour = 3600000;
    //localStorage.setItem("goose-games-expiry", (new Date().getTime() + numberOfMillisecondsPerHour).toString());

    if (playerName) {
      localStorage.setItem("goose-games-player-name", playerName);
    }
  }

  GetPlayerDetails(): IPlayerSession {
    var expiry = localStorage.getItem("goose-games-expiry");

    if (!expiry || Number(expiry) > new Date().getTime()) {

      var sessionId = localStorage.getItem("goose-games-session-id");
      var playerId = localStorage.getItem("goose-games-player-id");

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
    localStorage.removeItem("goose-games-session-id");
    localStorage.removeItem("goose-games-player-id");
    localStorage.removeItem("goose-games-expiry");
    localStorage.removeItem("goose-games-player-name");
  }
}
