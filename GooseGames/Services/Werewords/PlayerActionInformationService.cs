using Entities.Werewords.Enums;
using GooseGames.Logging;
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
        private readonly IPlayerRepository _playerRepository;
        private readonly RequestLogger<PlayerActionInformationService> _logger;

        public PlayerActionInformationService(IPlayerRepository playerRepository, RequestLogger<PlayerActionInformationService> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetAwakePlayersAsync(PlayerSessionRequest request)
        {
            return await GetPlayerInfoForPlayerStatus(request, PlayerStatusEnum.NightWaitingToWake);
        }

        private async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerInfoForPlayerStatus(PlayerSessionRequest request, Guid playerStatus)
        {
            _logger.LogTrace($"Getting player info for status {PlayerStatusEnum.GetDescription(playerStatus)}", request);

            _logger.LogTrace("Getting players for session");
            var playersToSelect = await _playerRepository.FilterAsync(p => p.SessionId == request.SessionId);
            var playerIds = playersToSelect.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting player status");
            var playersMatchingStatus = playersToSelect.Where(p => p.Status == playerStatus);

            _logger.LogTrace("Preparing result");
            var result = playersToSelect.Select(p => new PlayerActionResponse
            {
                Id = p.Id,
                PlayerName = p.Name,
                PlayerNumber = p.PlayerNumber,
                HasTakenAction = playersMatchingStatus.Any(x => x.Id == p.Id)
            });

            return GenericResponse<IEnumerable<PlayerActionResponse>>.Ok(result);
        }
    }
}
