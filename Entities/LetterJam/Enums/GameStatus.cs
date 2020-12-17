using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam.Enums
{
    public enum GameStatus
    {
        PreparingStartingWords = 1,
        ProposingClues = 2,
        ReceivedClue = 3,
        PreparingFinalWords = 4,
        RevealingFinalWords = 5,
        Finished = 6
    }
}
