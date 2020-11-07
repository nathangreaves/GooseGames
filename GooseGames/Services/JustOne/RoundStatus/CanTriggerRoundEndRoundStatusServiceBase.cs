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
        private readonly IGameRepository _gameRepository;
        private readonly Global.SessionService _sessionService;
        protected readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly JustOneHubContext _playerHub;

        public CanTriggerRoundEndRoundStatusServiceBase(IRoundRepository roundRepository,
            IGameRepository gameRepository,
            Global.SessionService sessionService,
            IPlayerStatusRepository playerStatusRepository,
            JustOneHubContext playerHub)
        {
            _roundRepository = roundRepository;
            _gameRepository = gameRepository;
            _sessionService = sessionService;
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
                var roundsRemoved = await _roundRepository.RemoveRoundsForGameAsync(round.GameId, Math.Abs(roundsToRemove));
                score = score + roundsRemoved;
            }

            var sessionEnded = (await _roundRepository.CountAsync(r => r.Status == RoundStatusEnum.New && r.GameId == round.GameId)) <= 0;

            var game = await _gameRepository.GetAsync(round.GameId);
            if (score != 0)
            {
                game.Score = game.Score + score;

                if (sessionEnded)
                {
                    await _sessionService.SetToLobbyAsync(game.SessionId);
                }

                await _gameRepository.UpdateAsync(game);
            }

            await _playerStatusRepository.UpdatePlayerStatusesForRoundAsync(round.Id, PlayerStatusEnum.PassivePlayerOutcome, PlayerStatusEnum.ActivePlayerOutcome);

            await _playerHub.SendRoundOutcomeAvailableAsync(game.SessionId);
        }
    }
}
