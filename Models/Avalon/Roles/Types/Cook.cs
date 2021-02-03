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

        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            Player player1 = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayerId }, players);
            Player player2 = GetRandomEvilPlayerExcept(new List<Guid> { currentPlayerId }, players);

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
            }.OrderBy(x => s_Random.Next()).ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 1;
        }
    }
}
