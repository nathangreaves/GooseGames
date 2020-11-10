export class FujiCard {
  faceValue: number;
}
export class FujiPlayedCard extends FujiCard {  
  combinedValue: number;
  pushed: boolean;
  flushed: boolean;
}
export class FujiHandCard extends FujiCard {
  id: string;
  faceValue: number;
  selected: boolean;
}
export class ActivePlayerPlayedCardAnimation extends FujiCard {
  playerName: string;
  playerEmoji: string;
}
