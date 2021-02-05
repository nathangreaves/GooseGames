using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class EvilRoleWithStandardInfo : EvilRoleBase
    {
        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return StandardEvilIntel(currentPlayer, players);
        }

    }
}
