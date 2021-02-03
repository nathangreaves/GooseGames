using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Sage : EvilRoleWithStandardInfo
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Sage;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return -1;
        }
    }
}
