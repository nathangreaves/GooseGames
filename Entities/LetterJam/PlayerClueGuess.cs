using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class PlayerClueGuess : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid PlayerId { get; set; }
        public Guid ClueId { get; set; }        
        public char Letter { get; set; }
        public bool Eliminated { get; set; }
    }
}
