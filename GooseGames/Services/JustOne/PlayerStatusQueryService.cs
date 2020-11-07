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
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly RequestLogger<PlayerStatusQueryService> _logger;

        public PlayerStatusQueryService(IPlayerStatusRepository playerStatusRepository, RequestLogger<PlayerStatusQueryService> logger)
        {
            _playerStatusRepository = playerStatusRepository;
            _logger = logger;
        }

        public async Task<bool> AllPlayersMatchStatusForGameAsync(Guid gameId, Guid playerStatus, Guid? activePlayerIdToExclude = null)
        {
            var includeActivePlayer = !activePlayerIdToExclude.HasValue;

            _logger.LogTrace("Getting players for session");
            var players = await _playerStatusRepository.FilterAsync(p => p.GameId== gameId && (includeActivePlayer || p.PlayerId != activePlayerIdToExclude.Value));
            
            _logger.LogTrace("Getting player status");
            var playersMatchingStatus = players.Count(p => p.Status == playerStatus);

            _logger.LogTrace($"Players that match status {PlayerStatusEnum.GetDescription(playerStatus)} = {playersMatchingStatus}, total players = {players.Count}");
            return playersMatchingStatus == players.Count;
        }
    }
}
