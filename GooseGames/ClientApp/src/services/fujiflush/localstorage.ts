import { IPlayerSession } from "../../models/session";
import { Injectable } from "@angular/core";
import { GooseGamesLocalStorage } from "../localstorage";

@Injectable({
  providedIn: 'root',
})
export class FujiLocalStorage {

  _gooseGamesLocalStorage: GooseGamesLocalStorage;

  constructor(gooseGamesLocalStorage: GooseGamesLocalStorage)
  {
    this._gooseGamesLocalStorage = gooseGamesLocalStorage;
  }

  GetPlayerName() {
    return this._gooseGamesLocalStorage.GetPlayerName();
  }

  CachePlayerDetails(playerSession: IPlayerSession, playerName: string) {
    localStorage.setItem("fuji-session-id", playerSession.SessionId);
    localStorage.setItem("fuji-player-id", playerSession.PlayerId);
    const numberOfMillisecondsPerHour = 3600000;
    localStorage.setItem("fuji-expiry", (new Date().getTime() + numberOfMillisecondsPerHour).toString());

    this._gooseGamesLocalStorage.CachePlayerName(playerName);
  }

  GetPlayerDetails(): IPlayerSession {
    var expiry = localStorage.getItem("fuji-expiry");

    if (expiry && Number(expiry) > new Date().getTime()) {

      var sessionId = localStorage.getItem("fuji-session-id");
      var playerId = localStorage.getItem("fuji-player-id");

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
    localStorage.removeItem("fuji-session-id");
    localStorage.removeItem("fuji-player-id");
    localStorage.removeItem("fuji-expiry");
  }
}
