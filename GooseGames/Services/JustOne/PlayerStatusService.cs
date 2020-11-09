using Entities.Global.Enums;
using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Requests;
using Models.Requests.JustOne.Round;
using Models.Responses;
using Models.Responses.PlayerStatus;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PlayerStatusService
    {
        private readonly Global.SessionService _sessionService;
        private readonly PlayerService _playerService;
        private readonly GlobalPlayerStatusService _globalPlayerStatusService;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly RoundService _roundService;
        private readonly PlayerStatusQueryService _playerStatusQueryService;
        private readonly JustOneHubContext _playerHubContext;
        private readonly RequestLogger<PlayerStatusService> _logger;

        public PlayerStatusService(Global.SessionService sessionService,
            Global.PlayerService playerService,
            Global.GlobalPlayerStatusService globalPlayerStatusService,
            IPlayerStatusRepository playerStatusRepository,
            RoundService roundService,
            PlayerStatusQueryService playerStatusQueryService,
            JustOneHubContext playerHubContext,
            RequestLogger<PlayerStatusService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _globalPlayerStatusService = globalPlayerStatusService;
            _playerStatusRepository = playerStatusRepository;
            _roundService = roundService;
            _playerStatusQueryService = playerStatusQueryService;
            _playerHubContext = playerHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateGlobalPlayerStatusLobbyAsync(PlayerSessionRequest request)
        {
            var globalResponse = await _globalPlayerStatusService.ValidatePlayerStatusAsync(request, GameEnum.JustOne);
            if (!globalResponse.Success)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error(globalResponse.ErrorCode);
            }

            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("You are not part of this session");
            }

            if (session.Status != SessionStatusEnum.Lobby)
            {
                var normalValidate = await ValidatePlayerStatusAsync(request, Entities.JustOne.Enums.PlayerStatusEnum.InLobby);
                if (normalValidate.Success)
                {
                    return normalValidate;
                }
            }

            var player = await _playerService.GetAsync(request.PlayerId);

            if (player == null)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("Player not found");
            }

            if (player.SessionId != request.SessionId)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("You are not part of this session");
            }

            return GenericResponse<PlayerStatusValidationResponse>.Ok(new PlayerStatusValidationResponse
            {
                RequiredStatus = Entities.JustOne.Enums.PlayerStatusEnum.InLobby.ToString(),
                StatusCorrect = player.Status == Entities.Global.Enums.PlayerStatusEnum.Lobby || player.Status == Entities.Global.Enums.PlayerStatusEnum.Ready
            });
        }

        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionRequest request, Guid status)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("You are not part of this session");
            }

            if (session.Game != GameEnum.JustOne || session.GameSessionId == null)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("This player and session are not valid for an in progress game of just one");
            }

            var playerStatus = await _playerStatusRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId && p.GameId == session.GameSessionId.Value);
            if (playerStatus == null)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("You are not part of this session");
            }

            if (session.Status == SessionStatusEnum.Abandoned)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("This session was abandoned");
            }

            if (session.Game != GameEnum.JustOne || session.GameSessionId.GetValueOrDefault() != playerStatus.GameId)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("This player is not part of the in progress game");
            }

            var response = new PlayerStatusValidationResponse
            {
                RequiredStatus = Entities.JustOne.Enums.PlayerStatusEnum.GetDescription(playerStatus.Status),
                StatusCorrect = playerStatus.Status == status
            };
            return GenericResponse<PlayerStatusValidationResponse>.Ok(response);
        }

        public async Task UpdatePlayerStatusAsync(Guid playerId, Guid gameId, Guid newStatus)
        {
            await _playerStatusRepository.UpdateStatusAsync(playerId, gameId, newStatus);
        }

        public async Task UpdateAllPlayersForGameAsync(Guid gameId, Guid newStatus)
        {
            await _playerStatusRepository.UpdatePlayerStatusesForGame(gameId, newStatus);
        }

        internal async Task UpdatePlayerStatusToRoundWaitingAsync(PlayerSessionRoundRequest request)
        {
            var gameId = await _sessionService.GetGameIdAsync(request.SessionId, GameEnum.JustOne);

            await UpdatePlayerStatusAsync(request.PlayerId, gameId.Value, Entities.JustOne.Enums.PlayerStatusEnum.RoundWaiting);

            await _playerHubContext.SendPlayerReadyForRoundAsync(request.SessionId, request.PlayerId);

            await _roundService.ProgressRoundAsync(request.RoundId);
        }

        internal async Task<GenericResponseBase> UpdatePlayerStatusToPassivePlayerClueAsync(PlayerSessionRequest request)
        {
            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (await _playerStatusQueryService.AllPlayersMatchStatusForGameAsync(currentRound.GameId, Entities.JustOne.Enums.PlayerStatusEnum.PassivePlayerWaitingForClues, currentRound.ActivePlayerId))
            {
                return GenericResponseBase.Error("Unable to undo as all players have now finished");
            }
            await UpdatePlayerStatusAsync(request.PlayerId, currentRound.GameId, Entities.JustOne.Enums.PlayerStatusEnum.PassivePlayerClue);

            await _playerHubContext.SendClueRevokedAsync(request.SessionId, request.PlayerId);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> UpdatePlayerStatusToPassivePlayerClueVoteAsync(PlayerSessionRoundRequest request)
        {
            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (await _playerStatusQueryService.AllPlayersMatchStatusForGameAsync(currentRound.GameId, Entities.JustOne.Enums.PlayerStatusEnum.PassivePlayerWaitingForClueVotes, currentRound.ActivePlayerId))
            {
                return GenericResponseBase.Error("Unable to undo as all players have now finished");
            }
            await UpdatePlayerStatusAsync(request.PlayerId, currentRound.GameId, Entities.JustOne.Enums.PlayerStatusEnum.PassivePlayerClueVote);

            await _playerHubContext.SendClueVoteRevokedAsync(request.SessionId, request.PlayerId);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> UpdatePlayerStatusToPassivePlayerOutcomeVoteAsync(PlayerSessionRoundRequest request)
        {
            var currentRound = await _roundService.GetCurrentRoundAsync(request);

            if (await _playerStatusQueryService.AllPlayersMatchStatusForGameAsync(currentRound.GameId, Entities.JustOne.Enums.PlayerStatusEnum.PassivePlayerWaitingForOutcomeVotes, currentRound.ActivePlayerId))
            {
                return GenericResponseBase.Error("Unable to undo as all players have now finished");
            }
            await UpdatePlayerStatusAsync(request.PlayerId, currentRound.GameId, Entities.JustOne.Enums.PlayerStatusEnum.PassivePlayerOutcomeVote);

            await _playerHubContext.SendResponseVoteRevokedAsync(request.SessionId, request.PlayerId);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> SetLobbyAsync(PlayerSessionRequest request)
        {
            return await _playerService.SendPlayerToLobbyAsync(request);
        }
    }
}
