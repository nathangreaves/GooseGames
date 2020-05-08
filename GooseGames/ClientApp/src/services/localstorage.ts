import { Injectable } from "@angular/core";

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

  Clear() {
    localStorage.removeItem("goose-games-player-name");
  }
}
