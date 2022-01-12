using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Percival : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Percival;
        public override bool ViableForDrunkToMimic => true;

        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            return players
                .Where(x => x.ActualRoleEnum == GameRoleEnum.Merlin || x.ActualRoleEnum == GameRoleEnum.Morgana)
                .Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.AppearsAsMerlin))
                .ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            var merlinInPlay = rolesInPlay.Any(x => x is Merlin);
            var morganaInPlay = rolesInPlay.Any(x => x is Morgana);

            short weight = 0; //If neither Merlin or Morgana in play, Percival is essentially a LSOA.
            if (merlinInPlay)
            {
                weight += 3; //Makes Merlin more powerful. Morgana's offset will counterbalance this so no need to account for it here.
            }
            if (!merlinInPlay && morganaInPlay)
            {
                weight += 2; //Percival just gets to see 1 bad person appearing as Merlin, which, if Merlin not in play, means that person is Morgana.
            }

            return weight;
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
                .Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.AppearsAsMerlin))
                .ToList();
        }
    }
}
