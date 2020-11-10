using Entities.Werewords.Enums;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Requests;
using Models.Responses;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords
{
    public class PlayerActionInformationService
    {
        private readonly IPlayerRoundInformationRepository playerRoundInformationRepository;
        private readonly Global.PlayerService _playerService;
        private readonly SessionService _sessionService;
        private readonly RequestLogger<PlayerActionInformationService> _logger;

        public PlayerActionInformationService(IPlayerRoundInformationRepository playerRepository, 
            Global.PlayerService playerService,
            SessionService sessionService,
            RequestLogger<PlayerActionInformationService> logger)
        {
            playerRoundInformationRepository = playerRepository;
            _playerService = playerService;
            _sessionService = sessionService;
            _logger = logger;
        }

        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetAwakePlayersAsync(PlayerSessionRequest request)
        {
            return await GetPlayerInfoForPlayerStatus(request, PlayerStatusEnum.NightWaitingToWake);
        }

        private async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerInfoForPlayerStatus(PlayerSessionRequest request, Guid playerStatus)
        {
            _logger.LogTrace($"Getting player info for status {PlayerStatusEnum.GetDescription(playerStatus)}", request);
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<IEnumerable< PlayerActionResponse>>.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error("Session does not have current round");
            }

            _logger.LogTrace("Getting players for session");
            var playersToSelect = await _playerService.GetForSessionAsync(request.SessionId);
            var playerIds = playersToSelect.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersMatchingStatus = await playerRoundInformationRepository.FilterAsync(p => playerIds.Contains(p.PlayerId) && p.Status == playerStatus);

            _logger.LogTrace("Preparing result");
            var result = playersToSelect.Select(p => new PlayerActionResponse
            {
                Id = p.Id,
                PlayerName = p.Name,
                PlayerNumber = p.PlayerNumber,
                Emoji = p.Emoji,
                HasTakenAction = playersMatchingStatus.Any(x => x.PlayerId == p.Id)
            });

            return GenericResponse<IEnumerable<PlayerActionResponse>>.Ok(result);
        }
    }
}
