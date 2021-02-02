using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responses.LetterJam
{
    public class ClueLetterResponse
    {
        public Guid? CardId { get; set; }
        public char? Letter { get; set; }
        public bool BonusLetter { get; set; }
        public bool BonusLetterGuessed { get; set; }
        public Guid? PlayerId { get; set; }
        public Guid? NonPlayerCharacterId { get; set; }
        public bool IsWildCard { get; set; }
    }
}
