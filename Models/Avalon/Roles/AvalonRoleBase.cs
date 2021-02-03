using Enums.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.Avalon.Roles
{
    public abstract class AvalonRoleBase
    {
        internal static readonly Random s_Random = new Random();
        public abstract List<PlayerIntel> GeneratePlayerIntel(Guid currentPlayerId, List<Player> players);
        public abstract bool AppearsEvilToMerlin { get; }
        public abstract bool AppearsEvilToEvil { get; }
        public abstract short GetRoleWeight(int numberOfPlayers);
        public abstract GameRoleEnum RoleEnum { get; }

        public List<PlayerIntel> StandardEvilIntel(Guid currentPlayerId, List<Player> players)
        {
            return players.Where(x => x.Role.AppearsEvilToEvil).Select(ConvertPlayerToPlayerIntel(currentPlayerId, IntelTypeEnum.AppearsEvil)).ToList();
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

        private static List<Player> GetGoodPlayersExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            return players.Where(x => !excludingPlayerIds.Contains(x.PlayerId) && x.Role is GoodRoleBase).ToList();
        }
        private static List<Player> GetSeenByMerlinAsEvilExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            return players.Where(x => !excludingPlayerIds.Contains(x.PlayerId) && x.Role.AppearsEvilToMerlin).ToList();
        }
        private static List<Player> GetEvilPlayersExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            return players.Where(x => !excludingPlayerIds.Contains(x.PlayerId) && x.Role is EvilRoleBase).ToList();
        }
        private static List<Player> GetPlayersExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            return players.Where(x => !excludingPlayerIds.Contains(x.PlayerId)).ToList();
        }

        internal static Player GetRandomSeenByMerlinAsEvilExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            var playerList = GetSeenByMerlinAsEvilExcept(excludingPlayerIds, players);

            var player = playerList[s_Random.Next(0, playerList.Count)];

            return player;
        }
        internal static Player GetRandomGoodPlayerExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            var goodPlayers = GetGoodPlayersExcept(excludingPlayerIds, players);

            var goodPlayer = goodPlayers[s_Random.Next(0, goodPlayers.Count)];
            return goodPlayer;
        }
        internal static Player GetRandomEvilPlayerExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            var evilPlayers = GetEvilPlayersExcept(excludingPlayerIds, players);

            var evilPlayer = evilPlayers[s_Random.Next(0, evilPlayers.Count)];

            return evilPlayer;
        }
        internal static Player GetRandomPlayerExcept(List<Guid> excludingPlayerIds, List<Player> players)
        {
            var playersExcept = GetPlayersExcept(excludingPlayerIds, players);

            var player = playersExcept[s_Random.Next(0, playersExcept.Count)];
            return player;
        }
    }
}
