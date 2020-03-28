using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using Models.Requests.JustOne;
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
        private readonly RequestLogger<PlayerDetailsService> _logger;

        public PlayerStatusService(IPlayerStatusRepository playerStatusRepository, RequestLogger<PlayerDetailsService> logger)
        {
            _playerStatusRepository = playerStatusRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePlayerStatusAsync(PlayerSessionRequest request, Guid status)
        {
            var playerStatus = await _playerStatusRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId);
            if (playerStatus == null)
            {
                return NewResponse.Error<PlayerStatusValidationResponse>("You are not part of this session");
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
            IEnumerable<PlayerStatus> playersToUpdate = new List<PlayerStatus>();

            await _playerStatusRepository.UpdatePlayerStatusesForSession(sessionId, newStatus);
        }
    }
}
