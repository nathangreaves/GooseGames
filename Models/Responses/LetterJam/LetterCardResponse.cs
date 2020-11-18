using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class LetterCardResponse
    {
        public Guid CardId { get; set; }
        public char Letter { get; set; }
        public bool BonusLetter { get; set; }
        public Guid? PlayerId { get; set; }
        public Guid? NonPlayerCharacterId { get; set; }
    }
}
