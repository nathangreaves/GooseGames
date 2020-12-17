using Entities.Common;
using Entities.LetterJam.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.LetterJam
{
    public class Round : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public int RoundNumber { get; set; }
        public RoundStatus RoundStatus { get; set; }
        public Guid? ClueGiverPlayerId { get; set; }
        public Guid? ClueId { get; set; }
        //public Clue Clue { get; set; }
    }
}
