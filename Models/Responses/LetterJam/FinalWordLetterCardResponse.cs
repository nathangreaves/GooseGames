using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class FinalWordLetterCardResponse
    {
        public Guid? CardId { get; set; }
        public bool IsWildCard { get; set; }
    }
}
