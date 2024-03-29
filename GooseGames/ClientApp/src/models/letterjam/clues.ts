import { IGooseGamesPlayer } from "../player";

export interface IProposedClueBase {
  id: string;
  playerId: string;
  numberOfLetters: number;
  numberOfPlayerLetters: number;
  numberOfNonPlayerLetters: number;
  wildcardUsed: boolean;
  numberOfBonusLetters: number;
}

export enum RoundStatusEnum {

  ProposingClues = 1,
  ReceivedClue = 2,
  GameEndTriggered = 3,
  GameEnd = 4
}

export interface IProposedClues {
  clues: IProposedClue[];
  roundStatus: RoundStatusEnum;
}

export interface IProposedClue {
  votes: IProposedClueVote[];
}

export interface IProposedClueVote {
  id: string;
  playerId: string;
}

export interface IClueLetter {
  cardId: string;
  letter: string;
  bonusLetter: boolean;
  bonusLetterGuessed: boolean;
  playerId: string;
  nonPlayerCharacterId: string;
  isWildCard: boolean;
}

export class ClueLetter implements IClueLetter {
  cardId: string;
  letter: string;
  bonusLetter: boolean;
  bonusLetterGuessed: boolean;
  playerId: string;
  nonPlayerCharacterId: string;
  isWildCard: boolean;
  player: IGooseGamesPlayer;
  loadingPlayer: boolean;
}

export class IClue {
  id: string;
  letters: ClueLetter[];
}

export class ProposedClue implements IProposedClueBase {
  id: string;
  playerId: string;
  numberOfLetters: number;
  numberOfPlayerLetters: number;
  numberOfNonPlayerLetters: number;
  wildcardUsed: boolean;
  numberOfBonusLetters: number;
  player: IGooseGamesPlayer;
  votes: ProposedClueVote[];
  letters: ClueLetter[];
  myClue: boolean;
  loadingPlayer: boolean;
  voted: boolean;
}

export class ProposedClueVote implements IProposedClueVote {
  id: string;
  playerId: string;
  player: IGooseGamesPlayer;
  loadingPlayer: boolean;
}
