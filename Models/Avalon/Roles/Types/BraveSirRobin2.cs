using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class BraveSirRobin2 : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.BraveSirRobin2;

        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            var player = GetRandomSeenByMerlinAsEvilExcept(new List<Guid> { currentPlayerId }, players);

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.AppearsEvil,
                    PlayerId = currentPlayerId,
                    IntelPlayerId = player.PlayerId
                },
                new PlayerIntel
                {
                    IntelType = IntelTypeEnum.RoleSeesYouAsEvil,
                    PlayerId = player.PlayerId,
                    RoleKnowsYou = GameRoleEnum.BraveSirRobin2
                }
            };
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 0;
        }
    }
}
