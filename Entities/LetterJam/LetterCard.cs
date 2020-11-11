using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class LetterCard : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public char Letter { get; set; }
        public Guid? PlayerId { get; set; }
        public Guid? NonPlayerCharacterId { get; set; }
        public int? LetterIndex { get; set; }
        public char? PlayerLetterGuess { get; set; }
        public bool BonusLetter { get; set; }
    }
}
