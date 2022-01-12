using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Oberon : EvilRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Oberon;
        public override bool AppearsEvilToEvil => false;

        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return new List<PlayerIntel>();
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 1;
        }
    }
}
