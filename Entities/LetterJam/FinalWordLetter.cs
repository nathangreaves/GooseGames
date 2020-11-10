using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class FinalWordLetter : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid? PlayerId { get; set; }
        public int LetterNumber { get; set; }
        public char Letter { get; set; }
        public char? PlayerLetterGuess { get; set; }
        public bool BonusLetter { get; set; }
        public bool Wildcard { get; set; }
    }
}
