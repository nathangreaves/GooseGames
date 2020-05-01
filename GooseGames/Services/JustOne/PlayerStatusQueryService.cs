using Entities.JustOne.Enums;
using GooseGames.Logging;
using RepositoryInterface.JustOne;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PlayerStatusQueryService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly RequestLogger<PlayerStatusQueryService> _logger;

        public PlayerStatusQueryService(IPlayerRepository playerRepository, IPlayerStatusRepository playerStatusRepository, RequestLogger<PlayerStatusQueryService> logger)
        {
            _playerRepository = playerRepository;
            _playerStatusRepository = playerStatusRepository;
            _logger = logger;
        }

        public async Task<bool> AllPlayersMatchStatus(Guid sessionId, Guid playerStatus, Guid? activePlayerIdToExclude = null)
        {
            var includeActivePlayer = !activePlayerIdToExclude.HasValue;

            _logger.LogTrace("Getting players for session");
            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId && (includeActivePlayer || p.Id != activePlayerIdToExclude.Value));
            var playerIds = players.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersMatchingStatus = await _playerStatusRepository.CountAsync(p => playerIds.Contains(p.PlayerId) && p.Status == playerStatus);

            _logger.LogTrace($"Players that match status {PlayerStatusEnum.GetDescription(playerStatus)} = {playersMatchingStatus}, total players = {players.Count}");
            return playersMatchingStatus == players.Count;
        }
    }
}
