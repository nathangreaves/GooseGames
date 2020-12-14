import { IGooseGamesPlayer } from "../player";
import { ILetterCard } from "./letters";
import { RoundStatusEnum } from "./clues";

export interface ITablePlayerBase {
  numberOfRedCluesGiven: number;
  numberOfGreenCluesGiven: number;
  numberOfLetters: number;
  currentLetterIndex: number | null;
  currentLetterId: string;
}

export interface INonPlayerCharacter {
  nonPlayerCharacterId: string;
  name: string;
  emoji: string;
  playerNumber: number;
}

export interface ITablePlayer extends ITablePlayerBase {
  playerId: string;
}

export interface ITableNonPlayerCharacterBase {

  numberOfLettersRemaining: number;
  currentLetterId: string;
  clueReleased: boolean;
}

export interface ITableNonPlayerCharacter extends ITableNonPlayerCharacterBase {
  nonPlayerCharacterId: string;
}

export interface ITable {
  currentRoundId: string;
  redCluesRemaining: number;
  greenCluesRemaining: number;
  lockedCluesRemaining: number;
  players: ITablePlayer[];
  nonPlayerCharacters: ITableNonPlayerCharacter[];
  bonusCardIds: string[];
}

export interface IRound {
  roundStatus: RoundStatusEnum,
  roundId: string;
  playerStatus: string;
}

export class TablePlayerBase  {
  playerId: string;
  currentLetterId: string;
  currentLetter: ILetterCard;
  player: IGooseGamesPlayer;
  loadingPlayer: boolean;
  loadingCard: boolean;
  cards: number[];
}

export class TablePlayer extends TablePlayerBase implements ITablePlayer {
  numberOfRedCluesGiven: number;
  numberOfGreenCluesGiven: number;
  numberOfLetters: number;
  currentLetterIndex: number | null;
}

export class TableNonPlayerCharacter extends TablePlayerBase implements ITableNonPlayerCharacterBase {
  numberOfLettersRemaining: number;  
  clueReleased: boolean;
}

export interface ICardsRequest {
  cardIds: string[] | null;
  relevantCards: boolean | null;
}

export interface ITokenUpdate {
  addRedTokenToPlayerId: string;
  addGreenTokenToPlayerId: string;
  unlockTokensFromNonPlayerCharacterIds: string[];
  unlockTokensFromSupply: number;
}
