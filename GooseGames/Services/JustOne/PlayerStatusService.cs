using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.Requests;
using Models.Requests.JustOne.Round;
using Models.Responses;
using Models.Responses.JustOne.PlayerStatus;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PlayerStatusService
    {
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly RoundService _roundService;
        private readonly ISessionRepository _sessionRepository;
        private readonly PlayerStatusQueryService _playerStatusQueryService;
        private readonly PlayerHubContext _playerHubContext;
        private readonly RequestLogger<PlayerDetailsService> _logger;

        public PlayerStatusService(IPlayerStatusRepository playerStatusRepository,
            RoundService roundService,
            ISessionRepository sessionRepository,
            PlayerStatusQueryService playerStatusQueryService,
            PlayerHubContext playerHubContext,
            RequestLogger<PlayerDetailsService> logger)
        {
            _playerStatusRepository = playerStatusRepository;
            _roundService = roundService;
            _sessionRepository = sessionRepository;
            _playerStatusQueryService = playerStatusQueryService;
            _playerHubContext = playerHubContext;
            _logger = logger;
        }

        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionRequest request, Guid status)
        {
            var playerStatus = await _playerStatusRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId);
            if (playerStatus == null)
            {
                return NewResponse.Error<PlayerStatusValidationResponse>("You are not part of this session");
            }

            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {
                return NewResponse.Error<PlayerStatusValidationResponse>("You are not part of this session");
            }

            if (session.StatusId == SessionStatusEnum.Abandoned)
            {
                return NewResponse.Error<PlayerStatusValidationResponse>("This session was abandoned");
            }

            var response = new PlayerStatusValidationResponse
            {
                RequiredStatus = PlayerStatusEnum.GetDescription(playerStatus.Status),
                StatusCorrect = playerStatus.Status == status
            };
            return NewResponse.Ok(response);
        }

        public async Task UpdatePlayerStatusAsync(Guid playerId, Guid newStatus)
        {
            var status = await _playerStatusRepository.SingleOrDefaultAsync(p => p.PlayerId == playerId);
            status.Status = newStatus;

            await _playerStatusRepository.UpdateAsync(status);
        }

        public async Task UpdateAllPlayersForSessionAsync(Guid sessionId, Guid newStatus)
        {
            await _playerStatusRepository.UpdatePlayerStatusesForSession(sessionId, newStatus);
        }

        internal async Task UpdatePlayerStatusToRoundWaitingAsync(PlayerSessionRoundRequest request)
        {
            await UpdatePlayerStatusAsync(request.PlayerId, PlayerStatusEnum.RoundWaiting);

            await _playerHubContext.SendPlayerReadyForRoundAsync(request.SessionId, request.PlayerId);

            await _roundService.ProgressRoundAsync(request.RoundId);
        }

        internal async Task<GenericResponseBase> UpdatePlayerStatusToPassivePlayerClueAsync(PlayerSessionRequest request)
        {
            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (await _playerStatusQueryService.AllPlayersMatchStatus(request.SessionId, PlayerStatusEnum.PassivePlayerWaitingForClues, currentRound.ActivePlayerId))
            {
                return GenericResponseBase.Error("Unable to undo as all players have now finished");
            }
            await UpdatePlayerStatusAsync(request.PlayerId, PlayerStatusEnum.PassivePlayerClue);

            await _playerHubContext.SendClueRevokedAsync(request.SessionId, request.PlayerId);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> UpdatePlayerStatusToPassivePlayerClueVoteAsync(PlayerSessionRoundRequest request)
        {
            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (await _playerStatusQueryService.AllPlayersMatchStatus(request.SessionId, PlayerStatusEnum.PassivePlayerWaitingForClueVotes, currentRound.ActivePlayerId))
            {
                return GenericResponseBase.Error("Unable to undo as all players have now finished");
            }
            await UpdatePlayerStatusAsync(request.PlayerId, PlayerStatusEnum.PassivePlayerClueVote);

            await _playerHubContext.SendClueVoteRevokedAsync(request.SessionId, request.PlayerId);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> UpdatePlayerStatusToPassivePlayerOutcomeVoteAsync(PlayerSessionRoundRequest request)
        {
            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (await _playerStatusQueryService.AllPlayersMatchStatus(request.SessionId, PlayerStatusEnum.PassivePlayerWaitingForOutcomeVotes, currentRound.ActivePlayerId))
            {
                return GenericResponseBase.Error("Unable to undo as all players have now finished");
            }
            await UpdatePlayerStatusAsync(request.PlayerId, PlayerStatusEnum.PassivePlayerOutcomeVote);

            await _playerHubContext.SendResponseVoteRevokedAsync(request.SessionId, request.PlayerId);

            return GenericResponseBase.Ok();
        }
    }
}
