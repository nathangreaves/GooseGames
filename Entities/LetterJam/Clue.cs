using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class Clue : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid RoundId { get; set; }
        public Round Round { get; set; }
        public int RoundNumber { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid ClueGiverPlayerId { get; set; }
        public bool ClueSuccessful { get; set; }
        public int NumberOfLetters { get; set; }
        public int NumberOfPlayerLetters { get; set; }
        public int NumberOfNonPlayerLetters { get; set; }
        public bool WildcardUsed { get; set; }
        public int NumberOfBonusLetters { get; set; }
    }
}
