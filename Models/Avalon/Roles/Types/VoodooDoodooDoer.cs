using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class VoodooDoodooDoer : EvilRoleWithStandardInfo
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.VoodooDoodooDoer;

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return -2;
        }
    }
}
