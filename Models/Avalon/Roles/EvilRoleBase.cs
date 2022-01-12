using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class EvilRoleBase : AvalonRoleBase
    {
        public override bool AppearsEvilToMerlin => true;
        public override bool AppearsEvilToEvil => true;
        public override bool ViableForDrunkToMimic => false;
        public override bool ViableForMyopiaInfo => false;
        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return new List<PlayerIntel>();
        }
    }
}
