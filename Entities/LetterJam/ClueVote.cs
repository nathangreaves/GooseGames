using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class ClueVote : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid ClueId { get; set; }
        public Clue Clue { get; set; }
        public Guid RoundId { get; set; }
        public Round Round { get; set; }
        public Guid PlayerId { get; set; }
    }
}
