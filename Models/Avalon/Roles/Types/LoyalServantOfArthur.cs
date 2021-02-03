using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class LoyalServantOfArthur : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.LoyalServantOfArthur;
        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            return new List<PlayerIntel>();
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 0;
        }
    }
}
