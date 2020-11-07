using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Microsoft.AspNetCore.SignalR;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne.RoundStatus
{
    public class NewRoundStatusService : RoundStatusKeyedServiceBase
    {
        private readonly PlayerService _playerService;
        private readonly IRoundRepository _roundRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly JustOneHubContext _playerHub;
        private readonly RequestLogger<NewRoundStatusService> _logger;

        public override RoundStatusEnum RoundStatus => RoundStatusEnum.New;

        public NewRoundStatusService(Global.PlayerService playerService, 
            IRoundRepository roundRepository, 
            IPlayerStatusRepository playerStatusRepository,
            JustOneHubContext playerHub,
            RequestLogger<NewRoundStatusService> logger)
        {
            _playerService = playerService;
            _roundRepository = roundRepository;
            _playerStatusRepository = playerStatusRepository;
            _playerHub = playerHub;
            _logger = logger;
        }

        public async Task UpdatePlayerStatusAsync(Round round)
        {
            var roundId = round.Id;
            var gameId = round.GameId;
            var sessionId = round.SessionId;

            _logger.LogTrace($"Updating player statuses for round: {roundId}");
            await _playerStatusRepository.UpdatePlayerStatusesForRoundAsync(roundId, PlayerStatusEnum.PassivePlayerClue, PlayerStatusEnum.ActivePlayerWaitingForClues);

            _logger.LogTrace($"Getting the connection id for the active player");
            var activePlayerConnectionId = (await _playerStatusRepository.SingleOrDefaultAsync(p => p.GameId == round.GameId && p.PlayerId == round.ActivePlayerId.Value)).ConnectionId;

            _logger.LogTrace($"Sending begin round. Active player connection id={activePlayerConnectionId}");
            await _playerHub.SendBeginRoundAsync(sessionId, activePlayerConnectionId);
        }

        public async Task TransitionRoundStatusAsync(Round round)
         {
            var roundId = round.Id;

            round.Status = RoundStatusEnum.WaitingForResponses;

            _logger.LogTrace($"Updating round: {roundId}");
            await _roundRepository.UpdateAsync(round);

            await UpdatePlayerStatusAsync(round);
        }

        public override async Task ConditionallyTransitionRoundStatusAsync(Round round)
        {
            await TransitionRoundStatusAsync(round);
        }
    }
}
