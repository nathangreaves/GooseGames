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

export interface IProposedClue {
  votes: IProposedClueVote[];
}

export interface IProposedClueVote {
  id: string;
  playerId: string;
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
  loadingPlayer: boolean;
}

export class ProposedClueVote implements IProposedClueVote {
  id: string;
  playerId: string;
  player: IGooseGamesPlayer;
  loadingPlayer: boolean;
}
