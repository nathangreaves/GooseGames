class PlayerNumberCssResolver {
  GetClass(playerNumber: number) {
    return {
      'just-one-player-1': playerNumber == 1,
      'just-one-player-2': playerNumber == 2,
      'just-one-player-3': playerNumber == 3,
      'just-one-player-4': playerNumber == 4,
      'just-one-player-5': playerNumber == 5,
      'just-one-player-6': playerNumber == 6,
      'just-one-player-7': playerNumber == 7
    }
  }
}

export const PlayerNumberCss = new PlayerNumberCssResolver();
