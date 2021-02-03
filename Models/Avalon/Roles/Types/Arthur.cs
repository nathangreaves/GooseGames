using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Arthur : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Arthur;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 1;
        }
    }
}
