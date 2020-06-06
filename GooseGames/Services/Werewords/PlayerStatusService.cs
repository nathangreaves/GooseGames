using Entities.Global.Enums;
using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Services.Global;
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
        private readonly Global.SessionService _sessionService;
        private readonly GlobalPlayerStatusService _playerStatusService;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly PlayerStatusKeyedServiceProvider _playerStatusKeyedServiceProvider;

        public PlayerStatusService(
            Global.SessionService sessionService,
            Global.GlobalPlayerStatusService playerStatusService,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            PlayerStatusKeyedServiceProvider playerStatusKeyedServiceProvider)
        {
            _sessionService = sessionService;
            _playerStatusService = playerStatusService;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _playerStatusKeyedServiceProvider = playerStatusKeyedServiceProvider;
        }

        internal async Task UpdateAllPlayersForSessionAsync(Guid roundId, Guid playerStatus)
        {
            var players = await _playerRoundInformationRepository.FilterAsync(p => p.RoundId == roundId);

            foreach (var player in players)
            {
                player.Status = playerStatus;
            }

            await _playerRoundInformationRepository.UpdateRangeAsync(players);
        }


        internal async Task ConditionallyUpdateAllPlayersForSessionAsync(Guid roundId, Guid fromStatus, Guid toStatus)
        {
            var players = await _playerRoundInformationRepository.FilterAsync(p => p.RoundId == roundId && p.Status == fromStatus);

            foreach (var player in players)
            {
                player.Status = toStatus;
            }

            await _playerRoundInformationRepository.UpdateRangeAsync(players);
        }

        internal async Task UpdatePlayerToStatusAsync(Guid playerId, Guid roundId, Guid playerStatus)
        {
            var player = await _playerRoundInformationRepository.SingleOrDefaultAsync(p => p.PlayerId == playerId && p.RoundId == roundId);

            player.Status = playerStatus;

            await _playerRoundInformationRepository.UpdateAsync(player);
        }

        internal async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionRequest request, Guid status)
        {
            //var globalPlayer = 
            var globalResponse = await _playerStatusService.ValidatePlayerStatusAsync(request, GameEnum.Werewords);
            if (!globalResponse.Success)
            {
                return GenericResponse<PlayerStatusValidationResponse>.Error(globalResponse.ErrorCode);
            }
            var globalPlayerStatus = globalResponse.Data;

            Guid playerStatus;
            if (globalPlayerStatus == Entities.Global.Enums.PlayerStatusEnum.New)
            {
                playerStatus = Entities.Werewords.Enums.PlayerStatusEnum.New;
            }
            else if (globalPlayerStatus == Entities.Global.Enums.PlayerStatusEnum.Ready)
            {
                playerStatus = Entities.Werewords.Enums.PlayerStatusEnum.InLobby;
            }
            else
            {
                var session = await _sessionService.GetAsync(request.SessionId);

                var player = await _playerRoundInformationRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId && p.RoundId == session.GameSessionId.Value);

                playerStatus = player.Status;
            }

            //if (player == null)
            //{
            //    return GenericResponse<PlayerStatusValidationResponse>.Error("a530d7fa-f842-492b-a0fc-6473af1c907a");
            //}
            //if (player.RoundId == null)
            //{
            //    return GenericResponse<PlayerStatusValidationResponse>.Error("511c0fb3-7d49-4fdf-a1a7-b1281b5ada4b");
            //}

            //var session = await _sessionService.GetAsync(request.SessionId);
            //if (session == null)
            //{
            //    return GenericResponse<PlayerStatusValidationResponse>.Error("You are not part of this session");
            //}

            //if (session.Status == SessionStatusEnum.Abandoned)
            //{
            //    return GenericResponse<PlayerStatusValidationResponse>.Error("This session was abandoned");
            //}

            var response = new PlayerStatusValidationResponse
            {
                RequiredStatus = Entities.Werewords.Enums.PlayerStatusEnum.GetDescription(playerStatus),
                StatusCorrect = playerStatus == status
            };
            return GenericResponse<PlayerStatusValidationResponse>.Ok(response);
        }

        private static readonly List<Guid> s_ValidNightStatuses = new List<Guid>
        {
            Entities.Werewords.Enums.PlayerStatusEnum.NightRevealSecretRole,
            Entities.Werewords.Enums.PlayerStatusEnum.NightWaitingForMayor,
            Entities.Werewords.Enums.PlayerStatusEnum.NightMayorPickSecretWord,
            Entities.Werewords.Enums.PlayerStatusEnum.NightSecretWord,
            Entities.Werewords.Enums.PlayerStatusEnum.NightWaitingToWake
        };

        internal async Task<GenericResponse<string>> TransitionNightAsync(PlayerSessionRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<string>.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponse<string>.Error("Session does not have current round");
            }

            var playerRoundInformation = await _playerRoundInformationRepository.GetForPlayerAndRoundAsync(session.GameSessionId.Value, request.PlayerId);
            if (playerRoundInformation == null)
            {
                return GenericResponse<string>.Error("Player round info not found");
            }

            var currentPlayerStatus = playerRoundInformation.Status;
            if (!s_ValidNightStatuses.Contains(currentPlayerStatus))
            {
                return GenericResponse<string>.Error("Player not in night phase");
            }
            var service = _playerStatusKeyedServiceProvider.GetService(currentPlayerStatus);

            return await service.TransitionPlayerStatus(session.GameSessionId.Value, playerRoundInformation);
        }
    }
}
