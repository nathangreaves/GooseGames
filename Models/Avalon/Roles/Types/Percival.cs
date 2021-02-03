using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Percival : GoodRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Percival;

        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            return players
                .Where(x => x.RoleEnum == GameRoleEnum.Merlin || x.RoleEnum == GameRoleEnum.Morgana)
                .Select(ConvertPlayerToPlayerIntel(currentPlayerId, IntelTypeEnum.AppearsAsMerlin))
                .ToList();
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return 3;
        }
    }
}
