using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Yvain : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Yvain;

        public override bool AppearsEvilToMerlin => true;
        public override bool AppearsEvilToEvil => true;

        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            return new List<PlayerIntel>();
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 1;
        }
    }
}
