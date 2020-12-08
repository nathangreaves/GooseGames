using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class MyJamLetterGuessesRequest : PlayerSessionGameRequest
    {
        public IEnumerable<MyJamLetterGuess> LetterGuesses { get; set; }
        public bool MoveOnToNextLetter { get; set; }
    }

    public class MyJamLetterGuess
    {
        public Guid CardId { get; set; }
        public char? PlayerLetterGuess { get; set; }
    }
}
