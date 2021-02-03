using Entities.Common;
using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Avalon
{
    public class PlayerIntel : IHasGuidId, IHasCreatedUtc, IHasLastUpdatedUtc
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public Guid PlayerId { get; set; }
        public IntelTypeEnum IntelType { get; set; }
        public Guid? IntelPlayerId { get; set; }
        public int? IntelNumber { get; set; }
        public GameRoleEnum? RoleKnowsYou { get; set; }
    }
}
