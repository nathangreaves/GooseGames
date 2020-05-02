import { FujiCard, FujiHandCard } from "./card";

export class FujiHand {
  numberOfCards: number;
}

export class FujiConcealedHand extends FujiHand {
  cards: FujiCard[];
}

export class FujiPlayerHand extends FujiHand {
  cards: FujiHandCard[];
}
