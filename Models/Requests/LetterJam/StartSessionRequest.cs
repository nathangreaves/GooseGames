using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.LetterJam
{
    public class StartSessionRequest : PlayerSessionRequest
    {
        public int NumberOfLetters { get; set; }
    }
}
