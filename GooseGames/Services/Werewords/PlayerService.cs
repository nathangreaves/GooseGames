using Entities.Werewords;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Enums.Werewords;
using Models.Requests;
using Models.Responses;
using Models.Responses.Werewords;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Werewords
{
    public class PlayerService
    {
        private readonly SessionService _sessionService;
        private readonly Global.PlayerService _playerService;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly RequestLogger<PlayerService> _logger;

        public PlayerService(
            SessionService sessionService,
            Global.PlayerService playerService,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            RequestLogger<PlayerService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<PlayerSecretRoleResponse>> GetSecretRoleAsync(PlayerSessionRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);

            if (session == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Session did not exist");
            }

            if (session.GameSessionId == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Session current round did not exist");
            }
            var playerInformation = await _playerRoundInformationRepository.SingleOrDefaultAsync(i => i.PlayerId == request.PlayerId && i.RoundId == session.GameSessionId.Value);

            if (playerInformation == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Could not find player information");
            }

            Guid? mayorPlayerId = playerInformation.PlayerId;
            if (!playerInformation.IsMayor)
            {
                mayorPlayerId = (await _playerRoundInformationRepository.SingleOrDefaultAsync(i => i.RoundId == session.GameSessionId.Value && i.IsMayor))?.PlayerId;
            }

            if (mayorPlayerId == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Could not find the mayor!");
            }

            var mayorPlayer = await _playerService.GetAsync(mayorPlayerId.Value);

            return GenericResponse<PlayerSecretRoleResponse>.Ok(new PlayerSecretRoleResponse 
            {
                MayorName = mayorPlayer.Name,
                MayorPlayerId = mayorPlayerId.Value,
                MayorEmoji = mayorPlayer.Emoji,
                SecretRole = (SecretRole)(int)playerInformation.SecretRole
            });
        }
    }
}
