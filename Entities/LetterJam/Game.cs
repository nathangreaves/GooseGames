using Entities.Common;
using Entities.LetterJam.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class Game : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public int RedCluesRemaining { get; set; }
        public int GreenCluesRemaining { get; set; }    
        public int LockedCluesRemaining { get; set; }   //TODO: Locked By? Probs don't actually need
        public GameStatus GameStatus { get; set; }
        public int? Points { get; set; }

        //TODO: Players
        //TODO: NPCs
        //TODO: Bonus Letters
    }
}
