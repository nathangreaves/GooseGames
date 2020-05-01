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
        private readonly PlayerStatusQueryService _playerStatusQueryService;
        private readonly PrepareNextRoundService _roundService;
        private readonly NewRoundStatusService _newRoundStatusService;
        private readonly RequestLogger<LeaderResponseResolvedRoundStatusService> _logger;

        public LeaderResponseResolvedRoundStatusService(
            PlayerStatusQueryService playerStatusQueryService,
            PrepareNextRoundService roundService,
            NewRoundStatusService newRoundStatusService,
            RequestLogger<LeaderResponseResolvedRoundStatusService> logger)
        {
            _playerStatusQueryService = playerStatusQueryService;
            _roundService = roundService;
            _newRoundStatusService = newRoundStatusService;
            _logger = logger;
        }

        public override RoundStatusEnum RoundStatus => RoundStatusEnum.LeaderResponseResolved;

        public override async Task ConditionallyTransitionRoundStatusAsync(Round round)
        {
            _logger.LogTrace("Checking all players are ready for next round", round);
            if (await AllPlayersReady(round.SessionId))
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

        private async Task<bool> AllPlayersReady(Guid sessionId)
        {
            return await _playerStatusQueryService.AllPlayersMatchStatus(sessionId, PlayerStatusEnum.RoundWaiting);
        }
    }
}
