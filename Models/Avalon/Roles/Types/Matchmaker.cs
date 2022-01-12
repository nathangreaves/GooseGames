using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Matchmaker : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Matchmaker;
        public override bool ViableForDrunkToMimic => true;

        public override List<PlayerIntel> GeneratePlayerIntel(Player currentPlayer, List<Player> players, List<AvalonRoleBase> allRoles)
        {
            Player player1 = null;
            Player player2 = null;

            var randomBool = s_Random.Next(0, 2);
            if (randomBool == 0)
            {
                player1 = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);
                player2 = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayer.PlayerId, player1.PlayerId }, players);
            }
            else
            {
                player1 = GetRandomEvilPlayerExcept(new List<Guid> { currentPlayer.PlayerId }, players);
                player2 = GetRandomEvilPlayerExcept(new List<Guid> { currentPlayer.PlayerId, player1.PlayerId }, players);
            }

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
            };
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
