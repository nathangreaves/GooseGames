using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PrepareNextRoundService
    {
        private readonly IRoundRepository _roundRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly RequestLogger<PrepareNextRoundService> _logger;

        public PrepareNextRoundService(
            IRoundRepository roundRepository, 
            ISessionRepository sessionRepository,
            IPlayerRepository playerRepository,
            RequestLogger<PrepareNextRoundService> logger)
        {
            _roundRepository = roundRepository;
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }


        internal async Task<Round> PrepareNextRoundAsync(Guid sessionId, Guid? previourActivePlayerId = null)
        {
            _logger.LogTrace($"Fetching next available round for session {sessionId}");
            var nextAvailableRound = await _roundRepository.FirstOrDefaultAsync(r => r.SessionId == sessionId
            && r.ActivePlayerId == null
            && r.Status == RoundStatusEnum.New);

            if (nextAvailableRound == null)
            {
                _logger.LogWarning("No round found.");
                return null;
            }

            _logger.LogTrace($"Assigning round to session {sessionId}");
            var session = await _sessionRepository.GetAsync(sessionId);
            session.CurrentRoundId = nextAvailableRound.Id;
            await _sessionRepository.UpdateAsync(session);

            _logger.LogTrace("Getting New Active Player");
            var activePlayer = await GetActivePlayerAsync(sessionId, previourActivePlayerId);

            _logger.LogTrace($"Assigning Active Player: {nextAvailableRound.ActivePlayerId} to round: {nextAvailableRound.Id}");
            nextAvailableRound.ActivePlayerId = activePlayer.Id;
            await _roundRepository.UpdateAsync(nextAvailableRound);

            _logger.LogTrace($"Assigned Active Player: {nextAvailableRound.ActivePlayerId} to round: {nextAvailableRound.Id}");

            return nextAvailableRound;
        }

        private async Task<Player> GetActivePlayerAsync(Guid sessionId, Guid? previousActivePlayerId)
        {
            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId);

            Player previousActivePlayer = null;
            if (previousActivePlayerId == null)
            {
                previousActivePlayer = players[new Random().Next(players.Count)];
            }
            else
            {
                previousActivePlayer = players.Single(p => p.Id == previousActivePlayerId.Value);
            }

            var orderedList = players.OrderBy(x => x.PlayerNumber).ToList();

            if (orderedList.Last().Id == previousActivePlayer.Id)
            {
                return orderedList.First();
            }
            var indexOfPrevious = orderedList.IndexOf(previousActivePlayer);
            return orderedList[indexOfPrevious + 1];

        }
    }
}
