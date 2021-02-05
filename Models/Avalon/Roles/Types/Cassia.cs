using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Cassia : Guinevere
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Cassia;
        public override bool ViableForDrunkToMimic => false;
        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 1;
        }
    }

}
