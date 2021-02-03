using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Macy : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Macy;

        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            var goodPlayer = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayerId }, players);
            var otherPlayer = GetRandomPlayerExcept(new List<Guid> { currentPlayerId, goodPlayer.PlayerId }, players);

            var list = new List<Player>
            {
                goodPlayer,
                otherPlayer
            }.OrderBy(x => s_Random.Next());

            return list.Select(x => new PlayerIntel
            {
                IntelType = IntelTypeEnum.ContextDependant,
                IntelPlayerId = x.PlayerId,
                PlayerId = currentPlayerId
            }).ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 1;
        }
    }
}
