using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class PlayerState : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid? CurrentLetterId { get; set; }
        public LetterCard CurrentLetter { get; set; }
        public int? CurrentLetterNumber { get; set; }
        public int CluesGiven { get; set; }
        public int OriginalWordLength { get; set; }
        public int? FinalWordLength { get; set; }
        public bool? Successful { get; set; }
        public int? Points { get; set; }
    }
}
