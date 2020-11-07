using Entities.Global;
using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using GooseGames.Services.Global;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PrepareNextRoundService
    {
        private readonly Global.SessionService _sessionService;
        private readonly PlayerService _playerService;
        private readonly IRoundRepository _roundRepository;
        private readonly IGameRepository _gameRepository;
        private readonly RequestLogger<PrepareNextRoundService> _logger;

        public PrepareNextRoundService(
            Global.SessionService sessionService,
            PlayerService playerService,
            IRoundRepository roundRepository, 
            IGameRepository gameRepository,
            RequestLogger<PrepareNextRoundService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _roundRepository = roundRepository;
            _gameRepository = gameRepository;
            _logger = logger;
        }


        internal async Task<Round> PrepareGameNextRoundAsync(Guid gameId, Guid sessionId, Guid? previourActivePlayerId = null)
        {
            _logger.LogTrace($"Fetching next available round for session {gameId}");
            var nextAvailableRound = await _roundRepository.FirstOrDefaultAsync(r => r.GameId == gameId
            && r.ActivePlayerId == null
            && r.Status == RoundStatusEnum.New);

            if (nextAvailableRound == null)
            {
                _logger.LogWarning("No round found.");
                return null;
            }

            _logger.LogTrace($"Assigning round to session {gameId}");
            var session = await _gameRepository.GetAsync(gameId);
            session.CurrentRoundId = nextAvailableRound.Id;
            await _gameRepository.UpdateAsync(session);

            _logger.LogTrace("Getting New Active Player");
            var activePlayerId = await GetActivePlayerAsync(sessionId, previourActivePlayerId);

            _logger.LogTrace($"Assigning Active Player: {activePlayerId} to round: {nextAvailableRound.Id}");
            nextAvailableRound.ActivePlayerId = activePlayerId;
            await _roundRepository.UpdateAsync(nextAvailableRound);

            _logger.LogTrace($"Assigned Active Player: {nextAvailableRound.ActivePlayerId} to round: {nextAvailableRound.Id}");

            return nextAvailableRound;
        }

        private async Task<Guid> GetActivePlayerAsync(Guid sessionId, Guid? previousActivePlayerId)
        {
            return await _playerService.GetNextActivePlayerAsync(sessionId, previousActivePlayerId);
        }
    }
}
