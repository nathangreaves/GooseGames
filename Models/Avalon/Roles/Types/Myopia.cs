using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Myopia : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Myopia;
        public override bool ViableForDrunkToMimic => true;

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            short weight = 0;
            if (allRoles.Any(x => x.ViableForMyopiaInfo))
            {
                weight += 1;
            }
            return weight;
        }
    }
}
