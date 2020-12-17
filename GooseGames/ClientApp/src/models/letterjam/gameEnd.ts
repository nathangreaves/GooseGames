import { IGooseGamesPlayer } from "../player";

export interface IGameEnd {
  players: IGameEndPlayer[];
}

export interface IGameEndPlayer {
  playerId: string;
  finalWordLetters: IGameEndPlayerLetter[];
  unusedLetters: IGameEndPlayerLetter[];
}

export interface IGameEndPlayerLetter {
  cardId: string;
  bonusLetter: boolean;
  isWildCard: boolean;
  playerLetterGuess: string;
  letter: string;
}

export class GameEndPlayer implements IGameEndPlayer {
  playerId: string;
  finalWordLetters: IGameEndPlayerLetter[];
  unusedLetters: IGameEndPlayerLetter[];
  player: IGooseGamesPlayer;
  loadingPlayer: boolean = true;
}
