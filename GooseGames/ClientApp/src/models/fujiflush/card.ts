export class FujiCard {
  faceValue: number;
}
export class FujiPlayedCard extends FujiCard {  
  combinedValue: number;
}
export class FujiHandCard extends FujiCard {
  id: string;
  faceValue: number;
  selected: boolean;
}
