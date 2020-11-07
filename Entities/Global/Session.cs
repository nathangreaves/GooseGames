using Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Entities.Global.Enums;

namespace Entities.Global
{
    public class Session : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string Password { get; set; }
        public ICollection<Player> Players { get; set; }
        public SessionStatusEnum Status { get; set; }
        public Guid? SessionMasterId { get; set; }
        public Player SessionMaster { get; set; }
        public GameEnum? Game { get; set; }
        public Guid? GameSessionId { get; set; }
    }
}
