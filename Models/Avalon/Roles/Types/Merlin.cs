using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Merlin : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Merlin;
        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            return players.Where(x => x.Role.AppearsEvilToMerlin).Select(ConvertPlayerToPlayerIntel(currentPlayerId, IntelTypeEnum.AppearsEvil)).ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return +2;
        }
    }
}
