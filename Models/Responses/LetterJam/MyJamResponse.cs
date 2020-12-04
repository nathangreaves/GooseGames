using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class MyJamResponse
    {
        public IEnumerable<MyJamRound> Rounds { get; set; }
        public int NumberOfLetters { get; set; }
        public int? CurrentLetterIndex { get; set; }
        public IEnumerable<MyJamLetterCard> MyLetters { get; set; }
    }

    public class MyJamRound
    {
        public Guid ClueGiverPlayerId { get; set; }
        public Guid ClueId { get; set; }
    }

    public class MyJamLetterCard
    {
        public Guid CardId { get; set; }
        public char? PlayerLetterGuess { get; set; }
        public bool BonusLetter { get; set; }
    }
}
