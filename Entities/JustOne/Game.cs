using Entities.Common;
using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne
{
    public class Game : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Game()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid SessionId { get; set; }
        public ICollection<Round> Rounds { get; set; }
        public Guid? CurrentRoundId { get; set; }
        public Round CurrentRound { get; set; }
        public int Score { get; set; }
    }
}
