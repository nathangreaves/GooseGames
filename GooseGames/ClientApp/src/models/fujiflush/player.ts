import { FujiHand } from "./hand";
import { FujiPlayedCard } from "./card";

export class FujiPlayer {
  id: string;
  name: string;
  playerNumber: number;
  isActivePlayer: boolean;
  hand: FujiHand;
  playedCard: FujiPlayedCard;
}
