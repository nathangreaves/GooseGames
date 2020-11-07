using Entities.Common;
using Entities.Global.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Global
{
    public class Player : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid? SessionId { get; set; }
        public Session Session { get; set; }
        public int PlayerNumber { get; set; }
        public string Name { get; set; }
        public PlayerStatusEnum Status { get; set; }
    }
}
