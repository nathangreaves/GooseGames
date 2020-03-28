using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using GooseGames.Services.JustOne.RoundStatus;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class RoundService
    {
        private readonly IRoundRepository _roundRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly RoundServiceProvider _roundServiceProvider;
        private readonly RequestLogger<RoundService> _logger;

        private const int DefaultNumberOfRounds = 13;

        public RoundService(IRoundRepository roundRepository, 
            ISessionRepository sessionRepository, 
            IPlayerRepository playerRepository, 
            RoundServiceProvider roundServiceProvider,
            RequestLogger<RoundService> logger)
        {
            _roundRepository = roundRepository;
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _roundServiceProvider = roundServiceProvider;
            _logger = logger;
        }

        internal async Task PrepareRoundsAsync(Guid sessionId)
        {
            _logger.LogTrace("Preparing Round", sessionId);

            var session = await _sessionRepository.GetAsync(sessionId);

            _logger.LogTrace("Found session");

            var words = GetWords(session, DefaultNumberOfRounds);

            var rounds = words.Select(word => 
            {
                return new Round
                {
                    Session = session,
                    Status = RoundStatusEnum.New,
                    WordToGuess = word
                };
            });

            await _roundRepository.InsertRangeAsync(rounds);

            var firstRound = await _roundRepository.FirstOrDefaultAsync(r => r.SessionId == session.Id);

            _logger.LogTrace("Getting Leader");

            var activePlayer = await GetLeaderAsync(session.Id);

            _logger.LogTrace($"Assigning Leader: {firstRound.ActivePlayerId} to round: {firstRound.Id}");

            firstRound.ActivePlayerId = activePlayer.Id;
            await _roundRepository.UpdateAsync(firstRound);

            _logger.LogTrace($"Assigned Leader: {firstRound.ActivePlayerId} to round: {firstRound.Id}");

            var roundStatusService = _roundServiceProvider.GetService(firstRound.Status);
            await roundStatusService.TransitionRoundStatusAsync(firstRound.Id);
            await roundStatusService.UpdatePlayerStatusAsync(sessionId, firstRound.Id);
        }

        private async Task<Player> GetLeaderAsync(Guid sessionId, Guid? previousLeaderId = null)
        {
            //TODO Efficiency saving to be made here by getting only the properties we need (playerid, playernumber) from the repository.
            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId);

            Player previousLeader = null;
            if (previousLeaderId == null)
            {
                previousLeader = players[new Random().Next(players.Count)];
            }
            else
            {
                previousLeader = players.Single(p => p.Id == previousLeaderId.Value);
            }

            var orderedList = players.OrderBy(x => x.PlayerNumber).ToList();
                
            if (orderedList.Last().Id == previousLeader.Id)
            {
                return orderedList.First();
            }
            var indexOfPrevious = orderedList.IndexOf(previousLeader);
            return orderedList[indexOfPrevious + 1];

        }

        public List<string> GetWords(Session session, int numberOfWords)
        {
            return TemporaryWordsList.GetWords(numberOfWords);
        }
    }
}
