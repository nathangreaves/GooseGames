import { FujiHand } from "./hand";
import { FujiPlayedCard } from "./card";

export class FujiPlayer {
  id: string;
  name: string;
  playerNumber: number;
  emoji: string;
  isActivePlayer: boolean;
  hand: FujiHand;
  playedCard: FujiPlayedCard;
}
