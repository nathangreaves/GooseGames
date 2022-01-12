using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class GoodRoleWithNoInfoBase : GoodRoleBase
    {
        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return new List<PlayerIntel>();
        }
        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return new List<PlayerIntel>();
        }
    }
}
