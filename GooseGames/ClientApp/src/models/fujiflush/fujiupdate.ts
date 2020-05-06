
export class FujiPlayedCardUpdate {
  playerId: string;
  faceValue: number;
  combinedValue: number;
}


export class FujiDiscardUpdate {
  playerId: string;
}



export class FujiDrawUpdate {
  playerId: string;
  newCardId: string;
}


export class ActivePlayerUpdate {
  activePlayerId: string;
  discardedCards: FujiDiscardUpdate[];
}


export class GameVictoryUpdate {
  winningPlayers: string[];
}

export class FujiUpdate {
  playedCards: FujiPlayedCardUpdate[];
  discardedCards: FujiDiscardUpdate[];
  newDraws: FujiDrawUpdate[];
  activePlayerUpdate: ActivePlayerUpdate;
  gameVictoryUpdate: GameVictoryUpdate;
}
