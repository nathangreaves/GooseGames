using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne.RoundStatus
{
    public class WaitingForResponsesRoundStatusService : RoundStatusKeyedServiceBase
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly PlayerHubContext _playerHub;
        private readonly RequestLogger<WaitingForResponsesRoundStatusService> _logger;

        public override RoundStatusEnum RoundStatus => RoundStatusEnum.WaitingForResponses;

        public WaitingForResponsesRoundStatusService(
            IResponseRepository responseRepository, 
            IRoundRepository roundRepository, 
            IPlayerStatusRepository playerStatusRepository,
            IPlayerRepository playerRepository,
            PlayerHubContext playerHub,
            RequestLogger<WaitingForResponsesRoundStatusService> logger)
        {
            _responseRepository = responseRepository;
            _roundRepository = roundRepository;
            _playerStatusRepository = playerStatusRepository;
            _playerRepository = playerRepository;
            _playerHub = playerHub;
            _logger = logger;
        }

        public override async Task ConditionallyTransitionRoundStatusAsync(Round round)
        {
            _logger.LogTrace("Checking all players have responded for round", round);

            //Have all players given responses?
            if (await _responseRepository.AllPlayersHaveResponded(round))
            {
                _logger.LogTrace("All players have responded");

                await TransitionRoundStatusAsync(round);
            }
            else
            {
                _logger.LogTrace("Not all players have responded");
            }
        }

        private async Task TransitionRoundStatusAsync(Round round)
        {
            var allResponses = await _responseRepository.FilterAsync(r => r.RoundId == round.Id);

            var duplicateResponses = allResponses
                .GroupBy(r => r.Word.ToLower())
                .Where(r => r.Count() > 1 || r.Key.ToLower() == round.WordToGuess.ToLower())
                .SelectMany(r => r)
                .ToList();

            foreach (var response in duplicateResponses)
            {
                response.Status = ResponseStatusEnum.AutoInvalid;
            }
            await _responseRepository.UpdateRangeAsync(duplicateResponses);

            round.Status = RoundStatusEnum.WaitingForVotesOnDuplicates;
            await _roundRepository.UpdateAsync(round);

            await UpdatePlayerStatusAsync(round);
        }

        private async Task UpdatePlayerStatusAsync(Round round)
        {
            var roundId = round.Id;
            var sessionId = round.SessionId;

            _logger.LogTrace($"Updating player statuses for round: {roundId}");
            await _playerStatusRepository.UpdatePlayerStatusesForRoundAsync(roundId, PlayerStatusEnum.PassivePlayerClueVote, PlayerStatusEnum.ActivePlayerWaitingForVotes);
                     
            _logger.LogTrace($"Sending all clues submitted");
            await _playerHub.SendAllCluesSubmittedAsync(sessionId);
        }
    }
}
