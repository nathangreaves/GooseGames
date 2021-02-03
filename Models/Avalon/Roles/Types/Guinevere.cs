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

        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            var random = new Random();
            Player goodPlayer = GetRandomGoodPlayerExcept(new List<Guid> { currentPlayerId }, players);

            return new List<PlayerIntel>
            {
                new PlayerIntel
                {
                    IntelPlayerId = goodPlayer.PlayerId,
                    PlayerId = currentPlayerId,
                    IntelType = IntelTypeEnum.DefinitelyGood
                }
            };
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 1;
        }
    }
}
