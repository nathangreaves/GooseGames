import { IGooseGamesPlayer } from "../player";

export interface ILetterCard {
  cardId: string;
  letter: string;
  bonusLetter: boolean;
  playerId: string;
  nonPlayerCharacterId: string;
}

export class LetterCard implements ILetterCard {
  cardId: string;
  letter: string;
  bonusLetter: boolean;
  playerId: string;
  nonPlayerCharacterId: string;
  player: IGooseGamesPlayer;
  loadingPlayer: boolean;
}
