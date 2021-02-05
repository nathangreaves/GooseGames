using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Karenevere : EvilRoleWithStandardInfo
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Karenevere;

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 0;
        }
    }
}
