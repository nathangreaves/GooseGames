using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Morgana : EvilRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Morgana;
        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return StandardEvilIntel(currentPlayer, players);
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            var merlinInPlay = rolesInPlay.Any(x => x is Merlin);
            var percivalInPlay = rolesInPlay.Any(x => x is Percival);

            short weight = 0; //If Percival not in play, Morgana is effectively just a MOM
            if (merlinInPlay && percivalInPlay)
            {
                weight -= 3; //Negates Percival's power if Merlin in play
            }

            return weight;
        }
    }
}
