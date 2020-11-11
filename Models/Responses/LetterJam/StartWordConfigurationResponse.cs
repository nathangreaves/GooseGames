using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class StartWordConfigurationResponse
    {
        public Guid ForPlayerId { get; set; }
        public int NumberOfLetters { get; set; }
    }
}
