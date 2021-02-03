using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class AvalonRoleBase
    {
        public abstract List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players);
        public abstract bool KnownToMerlin { get; }
        public abstract bool KnownToEvil { get; }
        public abstract short GetRoleWeight(int numberOfPlayers);
        public abstract GameRoleEnum RoleEnum { get; }

        public List<PlayerIntel> StandardEvilIntel(Guid currentPlayerId, List<Player> players)
        {
            return players.Where(x => x.Role.KnownToEvil).Select(ConvertPlayerToPlayerIntel(currentPlayerId, IntelTypeEnum.AppearsEvil)).ToList();
        }

        public static Func<Player, PlayerIntel> ConvertPlayerToPlayerIntel(Guid currentPlayerId, IntelTypeEnum intelType)
        {
            return x => new PlayerIntel
            {
                PlayerId = currentPlayerId,
                IntelPlayerId = x.PlayerId,
                IntelType = intelType
            };
        }
    }
}
