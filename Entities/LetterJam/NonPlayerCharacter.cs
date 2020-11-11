using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class NonPlayerCharacter : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string Emoji { get; set; }
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public int NumberOfLettersRemaining { get; set; }
        public Guid? CurrentLetterId { get; set; }
        public LetterCard CurrentLetter { get; set; }
        public bool ClueReleased { get; set; }
    }
}
