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
    public class WaitingForLeaderResponseRoundStatusService : CanTriggerRoundEndRoundStatusServiceBase
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly IHubContext<PlayerHub> _playerHub;
        private readonly RequestLogger<WaitingForLeaderResponseRoundStatusService> _logger;

        public override RoundStatusEnum RoundStatus => RoundStatusEnum.WaitingForLeaderResponse;

        public WaitingForLeaderResponseRoundStatusService(IResponseRepository responseRepository,
            IRoundRepository roundRepository,
            IPlayerStatusRepository playerStatusRepository,
            ISessionRepository sessionRepository,
            IHubContext<PlayerHub> playerHub,
            RequestLogger<WaitingForLeaderResponseRoundStatusService> logger) 
            : base(roundRepository, sessionRepository, playerStatusRepository, playerHub)
        {
            _responseRepository = responseRepository;
            _roundRepository = roundRepository;
            _playerStatusRepository = playerStatusRepository;
            _playerHub = playerHub;
            _logger = logger;
        }

        public override async Task ConditionallyTransitionRoundStatusAsync(Round round)
        {
            var response = await _responseRepository.FirstOrDefaultAsync(r => r.PlayerId == round.ActivePlayerId && r.RoundId == round.Id);
            if (response != null && response.Status != ResponseStatusEnum.PassedActivePlayerResponse)
            {
                if (response.Word.ToLower() == round.WordToGuess.ToLower())
                {
                    await TransitionRoundStatusOnAutoSuccessAsync(round, response);
                }
                else
                {
                    await TransitionRoundStatusToVoteRequiredAsync(round);
                }
            }
            else
            {
                await TransitionRoundStatusOnPassAsync(round);
            }
        }

        private async Task TransitionRoundStatusToVoteRequiredAsync(Round round)
        {
            round.Status = RoundStatusEnum.WaitingForVotesOnLeaderResponse;
            
            await _roundRepository.UpdateAsync(round);

            await _playerStatusRepository.UpdatePlayerStatusesForRoundAsync(round.Id, PlayerStatusEnum.PassivePlayerOutcomeVote, PlayerStatusEnum.ActivePlayerWaitingForOutcomeVotes);

            await _playerHub.SendActivePlayerResponseVoteRequiredAsync(round.SessionId);
        }

        private async Task TransitionRoundStatusOnPassAsync(Round round)
        {
            //Passed
            int score = 0;
            var roundOutcome = RoundOutcomeEnum.Pass;

            await TransitionRoundWithOutcomeAsync(round, score, roundOutcome);
        }

        private async Task TransitionRoundStatusOnAutoSuccessAsync(Round round, Response response)
        {
            int score = 1;
            var roundOutcome = RoundOutcomeEnum.Success;

            response.Status = ResponseStatusEnum.AutoCorrectActivePlayerResponse;
            await _responseRepository.UpdateAsync(response);

            await TransitionRoundWithOutcomeAsync(round, score, roundOutcome);
        }
    }
}
