using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Cook : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Cook;

        public override bool ViableForDrunkToMimic => true;

        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            Player player1 = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);
            Player player2 = GetRandomEvilPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.ContextDependant,
                    PlayerId = currentPlayer.PlayerId,
                    IntelPlayerId = player1.PlayerId
                },
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.ContextDependant,
                    PlayerId = currentPlayer.PlayerId,
                    IntelPlayerId = player2.PlayerId
                }
            }.OrderBy(x => s_Random.Next()).ToList();
        }

        internal override List<PlayerIntel> GenerateDrunkIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            var player1 = GetRandomPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);
            var list = new List<Player>
            {
                player1,
                GetRandomPlayerExcept(new List<Guid> { currentPlayer.PlayerId, player1.PlayerId }, players)
            };

            return list
                .Select(ConvertPlayerToPlayerIntel(currentPlayer.PlayerId, IntelTypeEnum.ContextDependant))
                .ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers, IEnumerable<AvalonRoleBase> rolesInPlay, IEnumerable<AvalonRoleBase> allRoles)
        {
            return 1;
        }
    }
}
