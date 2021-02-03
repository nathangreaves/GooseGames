using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class EvilRoleWithStandardInfo : EvilRoleBase
    {
        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            return StandardEvilIntel(currentPlayerId, players);
        }

    }
}
