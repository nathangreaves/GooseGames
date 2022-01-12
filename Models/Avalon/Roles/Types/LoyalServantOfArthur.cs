using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class LoyalServantOfArthur : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.LoyalServantOfArthur;

        public override bool ViableForDrunkToMimic => false;
        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 0;
        }
    }
}
