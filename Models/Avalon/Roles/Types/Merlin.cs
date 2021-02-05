using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Merlin : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Merlin;
        public override bool ViableForDrunkToMimic => true;
        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return players.Where(x => x.ActualRole.AppearsEvilToMerlin).Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.AppearsEvil)).ToList();
        }

        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var count = GeneratePlayerIntel(currentPlayer, players, allRoles).Count;

            var listIds = new List<Guid> { currentPlayer.PlayerId };
            var infoPlayers = new List<Player>();
            for (int i = 0; i < count; i++)
            {
                var randomPlayer = GetRandomGoodPlayerExcept(listIds, players);
                infoPlayers.Add(randomPlayer);
                listIds.Add(randomPlayer.PlayerId);
            }

            return infoPlayers
                .Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.AppearsEvil))
                .ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            var assassinInPlay = allRoles.Any(x => x is Assassin || x is AssassinPlus);
            var yvainInPlay = rolesInPlay.Any(x => x is Yvain);

            short weight = 5;

            if (assassinInPlay)
            {
                weight -= 3; //Can't share knowledge
            }
            if (yvainInPlay)
            {
                weight -= 1; //Knowledge is incomplete
            }

            return weight;
        }
    }
}
