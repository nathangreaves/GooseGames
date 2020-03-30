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
    public class WaitingForVotesOnLeaderResponseRoundStatusService : CanTriggerRoundEndRoundStatusServiceBase
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IResponseVoteRepository _responseVoteRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IHubContext<PlayerHub> _playerHub;
        private readonly RequestLogger<WaitingForVotesOnLeaderResponseRoundStatusService> _logger;

        public override RoundStatusEnum RoundStatus => RoundStatusEnum.WaitingForVotesOnLeaderResponse;

        public WaitingForVotesOnLeaderResponseRoundStatusService(
            IResponseRepository responseRepository,
            IResponseVoteRepository responseVoteRepository,
            IRoundRepository roundRepository,
            ISessionRepository sessionRepository,
            IPlayerStatusRepository playerStatusRepository,
            IPlayerRepository playerRepository,
            IHubContext<PlayerHub> playerHub,
            RequestLogger<WaitingForVotesOnLeaderResponseRoundStatusService> logger) : base(roundRepository,
                sessionRepository, playerStatusRepository, playerHub)
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
            var playerStatus = PlayerStatusEnum.PassivePlayerWaitingForOutcomeVotes;

            _logger.LogTrace("Getting players for session");
            var playersExceptActivePlayer = await _playerRepository.FilterAsync(p => p.SessionId == round.SessionId && p.Id != round.ActivePlayerId);
            var playerIds = playersExceptActivePlayer.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersThatHaveVotedCount = await _playerStatusRepository.CountAsync(p => playerIds.Contains(p.PlayerId) && p.Status == playerStatus);

            _logger.LogTrace($"Players that have voted = {playersThatHaveVotedCount}, players to vote = {playersExceptActivePlayer.Count}");
            return playersThatHaveVotedCount == playersExceptActivePlayer.Count;
        }

        private async Task TransitionRoundStatusAsync(Round round)
        {
            _logger.LogTrace("Transitioning round status");

            _logger.LogTrace("Fetching responses");
            var response = await _responseRepository.SingleOrDefaultAsync(r => r.RoundId == round.Id && r.PlayerId == round.ActivePlayerId);

            _logger.LogTrace("Updating response status");
            await UpdateResponseStatusAsync(round, response);

            if (response.Status == ResponseStatusEnum.CorrectActivePlayerResponse)
            {
                await TransitionRoundStatusOnSuccessAsync(round);
            }
            else
            {
                await TransitionRoundStatusOnFailAsync(round);
            }
        }

        private async Task TransitionRoundStatusOnSuccessAsync(Round round)
        {
            int score = 1;
            var roundOutcome = RoundOutcomeEnum.Success;

            await TransitionRoundWithOutcomeAsync(round, score, roundOutcome);
        }

        private async Task TransitionRoundStatusOnFailAsync(Round round)
        {
            int score = -1;
            var roundOutcome = RoundOutcomeEnum.Fail;

            await TransitionRoundWithOutcomeAsync(round, score, roundOutcome);
        }

        private async Task UpdateResponseStatusAsync(Round round, Response response)
        {
            _logger.LogTrace("Getting number of players for voting");
            var numberOfPlayers = (await _playerRepository.CountAsync(p => p.SessionId == round.SessionId)) - 1;
            _logger.LogTrace($"Getting number of players for voting = {numberOfPlayers}");
            var numberOfVotesRequiredForValid = Math.Ceiling((double)numberOfPlayers / 2);
            _logger.LogTrace($"Number of votes required = {numberOfVotesRequiredForValid}");

            _logger.LogTrace($"Getting all response votes");
            var allResponseVotes = await _responseVoteRepository.GetNumberOfVotesPerResponseAsync(new List<Guid> { response.Id });
                        
            var votes = allResponseVotes[response.Id];
            _logger.LogTrace($"Response {response.Id} has {votes} votes");
            if (allResponseVotes[response.Id] < numberOfVotesRequiredForValid)            {

                _logger.LogTrace($"Response {response.Id} not valid");
                response.Status = ResponseStatusEnum.IncorrectActivePlayerResponse;
            }
            else
            {
                response.Status = ResponseStatusEnum.CorrectActivePlayerResponse;
                _logger.LogTrace($"Response {response.Id} is valid");
            }
            
            _logger.LogTrace($"Updating response with status");
            await _responseRepository.UpdateAsync(response);            
        }
    }
}
