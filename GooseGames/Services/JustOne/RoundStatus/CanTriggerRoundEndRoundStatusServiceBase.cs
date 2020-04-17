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

            var roundsToRemove = score;
            if (roundsToRemove < 0)
            {
                var roundsRemoved = await _roundRepository.RemoveRoundsAsync(round.SessionId, Math.Abs(roundsToRemove));
                score = score + roundsRemoved;
            }

            var sessionEnded = (await _roundRepository.CountAsync(r => r.Status == RoundStatusEnum.New && r.SessionId == round.SessionId)) <= 0;

            if (score != 0)
            {
                var session = await _sessionRepository.GetAsync(round.SessionId);
                session.Score = session.Score + score;

                if (sessionEnded)
                {
                    session.StatusId = SessionStatusEnum.Complete;
                }

                await _sessionRepository.UpdateAsync(session);
            }

            await _playerStatusRepository.UpdatePlayerStatusesForRoundAsync(round.Id, PlayerStatusEnum.PassivePlayerOutcome, PlayerStatusEnum.ActivePlayerOutcome);

            await _playerHub.SendRoundOutcomeAvailableAsync(round.SessionId);
        }
    }
}
