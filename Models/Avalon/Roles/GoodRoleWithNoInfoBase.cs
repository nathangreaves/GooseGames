using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class GoodRoleWithNoInfoBase : GoodRoleBase
    {
        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            return new List<PlayerIntel>();
        }
    }
}
