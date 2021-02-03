using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class AssassinPlus : Assassin
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.AssassinPlus;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return -3;
        }
    }
}
