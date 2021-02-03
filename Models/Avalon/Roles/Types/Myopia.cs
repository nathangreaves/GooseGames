using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Myopia : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Myopia;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 1;
        }
    }
}
