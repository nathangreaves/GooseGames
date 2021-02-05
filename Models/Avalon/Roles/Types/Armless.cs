using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Armless : EvilRoleWithStandardInfo
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Armless;

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 2;
        }
    }
}
