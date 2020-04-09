using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using Microsoft.AspNetCore.SignalR;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne.RoundStatus
{
    public abstract class CanTriggerRoundEndRoundStatusServiceBase : RoundStatusKeyedServiceBase
    {
        private readonly IRoundRepository _roundRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly PlayerHubContext _playerHub;

        public CanTriggerRoundEndRoundStatusServiceBase(IRoundRepository roundRepository,
            ISessionRepository sessionRepository,
            IPlayerStatusRepository playerStatusRepository,
            PlayerHubContext playerHub)
        {
            _roundRepository = roundRepository;
            _sessionRepository = sessionRepository;
            _playerStatusRepository = playerStatusRepository;
            _playerHub = playerHub;
        }

        internal async Task TransitionRoundWithOutcomeAsync(Round round, int score, RoundOutcomeEnum roundOutcome)
        {
            round.Status = RoundStatusEnum.LeaderResponseResolved;
            round.Outcome = roundOutcome;

            await _roundRepository.UpdateAsync(round);

            var session = await _sessionRepository.GetAsync(round.SessionId);
            session.Score = session.Score + score;

            var roundsToRemove = score;
            if (roundsToRemove < 0)
            {
                await _roundRepository.RemoveRoundsAsync(session.Id, Math.Abs(roundsToRemove));
            }

            await _playerStatusRepository.UpdatePlayerStatusesForRoundAsync(round.Id, PlayerStatusEnum.PassivePlayerOutcome, PlayerStatusEnum.ActivePlayerOutcome);

            await _playerHub.SendRoundOutcomeAvailableAsync(round.SessionId);
        }
    }
}
