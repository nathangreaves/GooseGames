using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon
{
    public class PlayerIntel
    {
        public Guid PlayerId { get; set; }
        public IntelTypeEnum IntelType { get; set; }
        public Guid? IntelPlayerId { get; set; }
        public int? IntelNumber { get; set; }
        public GameRoleEnum? RoleKnowsYou { get; set; }
    }
}
