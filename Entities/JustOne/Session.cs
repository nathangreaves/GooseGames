using Entities.Common;
using Entities.JustOne.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.JustOne
{
    public class Session : IHasGuidId, IHasCreatedUtc
    {
        public Session()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Password { get; set; }
        public ICollection<Player> Players { get; set; }
        public ICollection<Round> Rounds { get; set; }
        public SessionStatusEnum StatusId { get; set; }
        public Guid? CurrentRoundId { get; set; }
        public Round CurrentRound { get; set; }
        public Guid? SessionMasterId { get; set; }
        public Player SessionMaster { get; set; }
        public int Score { get; set; }
    }
}
