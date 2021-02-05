using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class VoodooDoodooDoer : EvilRoleWithStandardInfo
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.VoodooDoodooDoer;
        public override bool ViableForMyopiaInfo => true;

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return -2;
        }
    }
}
