using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Services.Werewords.PlayerStatus;
using Models.Requests;
using Models.Responses;
using Models.Responses.PlayerStatus;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords
{
    public class PlayerStatusService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly ProgressRoundService _progressRoundService;
        private readonly PlayerStatusKeyedServiceProvider _playerStatusKeyedServiceProvider;

        public PlayerStatusService(IPlayerRepository playerRepository,
            ISessionRepository sessionRepository,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            ProgressRoundService progressRoundService,
            PlayerStatusKeyedServiceProvider playerStatusKeyedServiceProvider)
        {
            _playerRepository = playerRepository;
            _sessionRepository = sessionRepository;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _progressRoundService = progressRoundService;
            _playerStatusKeyedServiceProvider = playerStatusKeyedServiceProvider;
        }

        internal async Task UpdateAllPlayersForSessionAsync(Guid sessionId, Guid playerStatus)
        {
            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId);

            foreach (var player in players)
            {
                player.Status = playerStatus;
            }

            await _playerRepository.UpdateRangeAsync(players);
        }


        internal async Task ConditionallyUpdateAllPlayersForSessionAsync(Guid sessionId, Guid fromStatus, Guid toStatus)
        {
            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId && p.Status == fromStatus);

            foreach (var player in players)
            {
                player.Status = toStatus;
            }

            await _playerRepository.UpdateRangeAsync(players);
        }

        internal async Task UpdatePlayerToStatusAsync(Guid playerId, Guid playerStatus)
        {
            var player = await _playerRepository.GetAsync(playerId);

            player.Status = playerStatus;

            await _playerRepository.UpdateAsync(player);
        }

        internal async Task UpdatePlayerToStatusAndProgressRoundAsync(PlayerSessionRequest request, Guid playerStatus)
        {
            await UpdatePlayerToStatusAsync(request.PlayerId, playerStatus);

            await _progressRoundService.ProgressRoundAsync(request.SessionId);
        }

        internal async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionRequest request, Guid status)
        {
            var player = await _playerRepository.SingleOrDefaultAsync(p => p.Id == request.PlayerId);
            if (player == null)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("a530d7fa-f842-492b-a0fc-6473af1c907a");
            }
            if (player.SessionId == null)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error("511c0fb3-7d49-4fdf-a1a7-b1281b5ada4b");
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
                RequiredStatus = PlayerStatusEnum.GetDescription(player.Status),
                StatusCorrect = player.Status == status
            };
            return NewResponse.Ok(response);
        }

        private static readonly List<Guid> s_ValidNightStatuses = new List<Guid>
        {
            PlayerStatusEnum.NightRevealSecretRole,
            PlayerStatusEnum.NightWaitingForMayor,
            PlayerStatusEnum.NightMayorPickSecretWord,
            PlayerStatusEnum.NightSecretWord,
            PlayerStatusEnum.NightWaitingToWake
        };

        internal async Task<GenericResponse<string>> TransitionNightAsync(PlayerSessionRequest request)
        {
            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<string>.Error("Unable to find session");
            }
            if (!session.CurrentRoundId.HasValue)
            {
                return GenericResponse<string>.Error("Session does not have current round");
            }

            var playerRoundInformation = await _playerRoundInformationRepository.GetForPlayerAndRoundAsync(session.CurrentRoundId.Value, request.PlayerId);
            if (playerRoundInformation == null)
            {
                return GenericResponse<string>.Error("Player round info not found");
            }

            var player = playerRoundInformation.Player;
            if (player == null)
            {
                return GenericResponse<string>.Error("Unable to find player");
            }

            var currentPlayerStatus = player.Status;
            if (!s_ValidNightStatuses.Contains(currentPlayerStatus))
            {
                return GenericResponse<string>.Error("Player not in night phase");
            }
            var service = _playerStatusKeyedServiceProvider.GetService(currentPlayerStatus);

            return await service.TransitionPlayerStatus(session, playerRoundInformation);
        }
    }
}
