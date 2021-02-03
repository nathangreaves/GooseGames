using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Cassia : Guinevere
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Cassia;
        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 2;
        }
    }

}
