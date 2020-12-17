using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class FinalWordRequest : PlayerSessionGameRequest
    {
        public IEnumerable<MyJamLetterGuess> LetterGuesses { get; set; }
        public IEnumerable<ClueLetterRequest> FinalWordLetters { get; set; }
    }
}
