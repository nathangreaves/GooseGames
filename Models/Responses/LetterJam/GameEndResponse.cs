using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class GameEndResponse
    {
        public IEnumerable<GameEndPlayer> Players { get; set; }
        public int CluesRemaining { get; set; }
    }

    public class GameEndPlayer
    {
        public Guid PlayerId { get; set; }
        public IEnumerable<GameEndPlayerLetter> FinalWordLetters { get; set; }
        public IEnumerable<GameEndPlayerLetter> OriginalWordLetters { get; set; }
        public IEnumerable<GameEndPlayerLetter> UnusedLetters { get; set; }
    }

    public class GameEndPlayerLetter
    {
        public Guid? CardId { get; set; }
        public bool BonusLetter { get; set; }
        public bool IsWildCard { get; set; }
        public char? PlayerLetterGuess { get; set; }
        public char? Letter { get; set; }
        public int? LetterIndex { get; set; }
    }
}