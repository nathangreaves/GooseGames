using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class SirHector : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.SirHector;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 0;
        }
    }
}
