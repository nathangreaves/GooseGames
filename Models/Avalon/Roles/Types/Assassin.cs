using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Avalon.Roles.Types
{
    public class Assassin : EvilRoleBase
    {
        public override GameRoleEnum RoleEnum => GameRoleEnum.Assassin;
        public override List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players)
        {
            return StandardEvilIntel(currentPlayerId, players);
        }

        public override short GetRoleWeight(int numberOfPlayers)
        {
            return -3;
        }
    }
}
