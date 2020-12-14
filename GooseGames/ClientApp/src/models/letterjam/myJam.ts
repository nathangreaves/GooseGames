import { IGooseGamesPlayer } from "../player";
import { ClueLetter, IClueLetter } from "./clues";

export interface IMyJam {
  rounds: IMyJamRound[];
  numberOfLetters: number;
  currentLetterIndex: number;
  myLetters: IMyJamLetterCard[];
}

export interface IMyJamRound {
  clueGiverPlayerId: string;
  clueId: string;
  requestingPlayerReceivedClue: boolean;
  letters: IClueLetter[];
}

export interface IMyJamLetterCard {
  cardId: string;
  playerLetterGuess: string;
  bonusLetter: boolean;
  isWildCard: boolean;
}

export class MyJamRound implements IMyJamRound {
  clueId: string;
  clueGiverPlayerId: string;
  player: IGooseGamesPlayer;
  letters: ClueLetter[];
  loadingPlayer: boolean;
  loadingLetters: boolean;
  requestingPlayerReceivedClue: boolean;
  letterIndex: number;
}

export interface IFinalWordPublicLetter extends IFinalWordLetter {  
  letter: string;
}

export interface ILetterGuess {
  cardId: string;
  playerLetterGuess: string;
}

export interface IFinalWordLetter {
  cardId: string;
  isWildCard: boolean;
}
