using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Mordred : EvilRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Mordred;
        public override bool AppearsEvilToMerlin => false;
        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return StandardEvilIntel(currentPlayer, players);
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            short weight = 0;

            var merlinInPlay = rolesInPlay.Any(x => x is Merlin);
            if (merlinInPlay)
            {
                return -2;
            }

            return weight;
        }
    }
}
