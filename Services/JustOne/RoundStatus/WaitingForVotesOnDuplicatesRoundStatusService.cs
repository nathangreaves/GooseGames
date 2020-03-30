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
    public class WaitingForVotesOnDuplicatesRoundStatusService : RoundStatusKeyedServiceBase
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IResponseVoteRepository _responseVoteRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IHubContext<PlayerHub> _playerHub;
        private readonly RequestLogger<WaitingForResponsesRoundStatusService> _logger;

        public override RoundStatusEnum RoundStatus => RoundStatusEnum.WaitingForVotesOnDuplicates;

        public WaitingForVotesOnDuplicatesRoundStatusService(
            IResponseRepository responseRepository,
            IResponseVoteRepository responseVoteRepository,
            IRoundRepository roundRepository,
            IPlayerStatusRepository playerStatusRepository,
            IPlayerRepository playerRepository,
            IHubContext<PlayerHub> playerHub,
            RequestLogger<WaitingForResponsesRoundStatusService> logger)
        {
            _responseRepository = responseRepository;
            _responseVoteRepository = responseVoteRepository;
            _roundRepository = roundRepository;
            _playerStatusRepository = playerStatusRepository;
            _playerRepository = playerRepository;
            _playerHub = playerHub;
            _logger = logger;
        }

        public override async Task ConditionallyTransitionRoundStatusAsync(Round round)
        {
            _logger.LogTrace("Checking all players have voted for round", round);

            if (await PlayersHaveAllVoted(round))
            {
                _logger.LogTrace("All players have voted");

                await TransitionRoundStatusAsync(round);
            }
            else
            {
                _logger.LogTrace("Not all players have voted");
            }
        }

        private async Task<bool> PlayersHaveAllVoted(Round round)
        {
            var playerStatus = PlayerStatusEnum.PassivePlayerWaitingForClueVotes;

            _logger.LogTrace("Getting players for session");
            var playersExceptActivePlayer = await _playerRepository.FilterAsync(p => p.SessionId == round.SessionId && p.Id != round.ActivePlayerId);
            var playerIds = playersExceptActivePlayer.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersThatHaveVotedCount = await _playerStatusRepository.CountAsync(p => playerIds.Contains(p.PlayerId) && p.Status == playerStatus);

            _logger.LogTrace($"Players that have voted = {playersThatHaveVotedCount}, players to vote = {playersExceptActivePlayer.Count}");
            return playersThatHaveVotedCount == playersExceptActivePlayer.Count;
        }

        public override async Task TransitionRoundStatusAsync(Round round)
        {
            _logger.LogTrace("Transitioning round status");

            _logger.LogTrace("Fetching responses");
            var allResponses = await _responseRepository.FilterAsync(r => r.RoundId == round.Id);

            _logger.LogTrace("Marking duplicate responses");
            await MarkInvalidResponses(round, allResponses);

            _logger.LogTrace($"Updating round status");
            round.Status = RoundStatusEnum.WaitingForLeaderResponse;
            await _roundRepository.UpdateAsync(round);

            _logger.LogTrace($"Updating players");
            await UpdatePlayerStatusAsync(round);
        }

        private async Task MarkInvalidResponses(Round round, List<Response> allResponses)
        {
            _logger.LogTrace("Getting number of players for voting");
            var numberOfPlayers = (await _playerRepository.CountAsync(p => p.SessionId == round.SessionId)) - 1;
            _logger.LogTrace($"Getting number of players for voting = {numberOfPlayers}");
            var numberOfVotesRequiredForValid = Math.Ceiling((double)numberOfPlayers / 2);
            _logger.LogTrace($"Number of votes required = {numberOfVotesRequiredForValid}");

            _logger.LogTrace($"Getting all response votes");
            var allResponseVotes = await _responseVoteRepository.GetNumberOfVotesPerResponseAsync(allResponses.Select(r => r.Id));

            var responsesToUpdate = new List<Response>();

            foreach (var response in allResponses)
            {
                var votes = allResponseVotes[response.Id];
                _logger.LogTrace($"Response {response.Id} has {votes} votes");
                if (allResponseVotes[response.Id] < numberOfVotesRequiredForValid)
                {
                    if (response.Status != ResponseStatusEnum.AutoInvalid)
                    {
                        _logger.LogTrace($"Response {response.Id} not valid");
                        responsesToUpdate.Add(response);
                        response.Status = ResponseStatusEnum.ManualInvalid;
                    }
                    else
                    {
                        _logger.LogTrace($"Response {response.Id} auto invalid");
                    }
                }
                else
                {
                    responsesToUpdate.Add(response);
                    response.Status = ResponseStatusEnum.Valid;
                    _logger.LogTrace($"Response {response.Id} is valid");
                }
            }

            _logger.LogTrace($"Responses invalid: {responsesToUpdate.Count}");
            if (responsesToUpdate.Any())
            {
                _logger.LogTrace($"Updating invalid responses");
                await _responseRepository.UpdateRangeAsync(responsesToUpdate);
            }
        }

        public override async Task UpdatePlayerStatusAsync(Round round)
        {
            var roundId = round.Id;
            var sessionId = round.SessionId;

            _logger.LogTrace($"Updating player statuses for round: {roundId}");
            await _playerStatusRepository.UpdatePlayerStatusesForRoundAsync(roundId, PlayerStatusEnum.PassivePlayerWaitingForActivePlayer, PlayerStatusEnum.ActivePlayerGuess);

            _logger.LogTrace($"Sending all clue votes submitted");
            await _playerHub.SendAllClueVotesSubmittedAsync(sessionId);
        }
    }
}
