using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class FinalWordPublicCardResponse
    {
        public Guid? CardId { get; set; }
        public char? Letter { get; set; }
        public bool IsWildCard { get; set; }
    }
}
