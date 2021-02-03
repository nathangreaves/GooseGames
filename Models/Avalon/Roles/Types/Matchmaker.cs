using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Matchmaker : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Matchmaker;

        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            Player player1 = null;
            Player player2 = null;

            var randomBool = s_Random.Next(0, 2);
            if (randomBool == 0)
            {
                player1 = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayerId }, players);
                player2 = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayerId, player1.PlayerId }, players);
            }
            else
            {
                player1 = GetRandomEvilPlayerExcept(new List<Guid> { currentPlayerId }, players);
                player2 = GetRandomEvilPlayerExcept(new List<Guid> { currentPlayerId, player1.PlayerId }, players);
            }

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.ContextDependant,
                    PlayerId = currentPlayerId,
                    IntelPlayerId = player1.PlayerId
                },
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.ContextDependant,
                    PlayerId = currentPlayerId,
                    IntelPlayerId = player2.PlayerId
                }
            };
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 1;
        }
    }
}
