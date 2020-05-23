using Entities.Common;
using Entities.Werewords.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Werewords
{
    public class Session : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string Password { get; set; }
        public ICollection<Player> Players { get; set; }
        public SessionStatusEnum StatusId { get; set; }
        public Guid? SessionMasterId { get; set; }
        public Player SessionMaster { get; set; }
        public Guid? CurrentRoundId { get; set; }
        public Round CurrentRound { get; set; }
    }
}
