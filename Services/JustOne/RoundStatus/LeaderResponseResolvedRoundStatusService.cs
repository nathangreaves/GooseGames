using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne.RoundStatus
{
    public class LeaderResponseResolvedRoundStatusService : RoundStatusKeyedServiceBase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly PrepareNextRoundService _roundService;
        private readonly NewRoundStatusService _newRoundStatusService;
        private readonly RequestLogger<LeaderResponseResolvedRoundStatusService> _logger;

        public LeaderResponseResolvedRoundStatusService(
            IPlayerRepository playerRepository,
            IPlayerStatusRepository playerStatusRepository,
            PrepareNextRoundService roundService,
            NewRoundStatusService newRoundStatusService,
            RequestLogger<LeaderResponseResolvedRoundStatusService> logger)
        {
            _playerRepository = playerRepository;
            _playerStatusRepository = playerStatusRepository;
            _roundService = roundService;
            _newRoundStatusService = newRoundStatusService;
            _logger = logger;
        }

        public override RoundStatusEnum RoundStatus => RoundStatusEnum.LeaderResponseResolved;

        public override async Task ConditionallyTransitionRoundStatusAsync(Round round)
        {
            _logger.LogTrace("Checking all players are ready for next round", round);
            if (await AllPlayersReady(round))
            {
                _logger.LogTrace("All players are ready for next round", round);
                await TransitionRoundStatusAsync(round);
            }
            else
            {
                _logger.LogTrace("Not all players are ready for next round");
            }
        }

        private async Task TransitionRoundStatusAsync(Round round)
        {
            var newRound = await _roundService.PrepareNextRoundAsync(round.SessionId, round.ActivePlayerId);

            await _newRoundStatusService.ConditionallyTransitionRoundStatusAsync(newRound);
        }

        private async Task<bool> AllPlayersReady(Round round)
        {
            var playerStatus = PlayerStatusEnum.RoundWaiting;

            _logger.LogTrace("Getting players for session");
            var playersExceptActivePlayer = await _playerRepository.FilterAsync(p => p.SessionId == round.SessionId);
            var playerIds = playersExceptActivePlayer.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersThatHaveVotedCount = await _playerStatusRepository.CountAsync(p => playerIds.Contains(p.PlayerId) && p.Status == playerStatus);

            _logger.LogTrace($"Players that have voted = {playersThatHaveVotedCount}, players to vote = {playersExceptActivePlayer.Count}");
            return playersThatHaveVotedCount == playersExceptActivePlayer.Count;
        }
    }
}
