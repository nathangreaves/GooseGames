using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Guinevere : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Guinevere;

        public override bool ViableForDrunkToMimic => true;

        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var random = new Random();
            Player goodPlayer = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelPlayerId = goodPlayer.PlayerId,
                    PlayerId = currentPlayer.PlayerId,
                    IntelType = IntelTypeEnum.DefinitelyGood
                }
            };
        }
        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var player1 = GetRandomPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);
            var list = new List<Player>
            {
                player1
            };

            return list
                .Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.DefinitelyGood))
                .ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 1;
        }
    }
}
