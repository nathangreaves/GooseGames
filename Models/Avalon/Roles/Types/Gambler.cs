using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Gambler : GoodRoleWithNoInfoBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Gambler;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return -2;
        }
    }
}
