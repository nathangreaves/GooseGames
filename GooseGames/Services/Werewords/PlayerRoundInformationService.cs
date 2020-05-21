using GooseGames.Logging;
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
    public class PlayerRoundInformationService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly RequestLogger<PlayerRoundInformationService> _logger;

        public PlayerRoundInformationService(ISessionRepository sessionRepository,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            RequestLogger<PlayerRoundInformationService> logger)
        {
            _sessionRepository = sessionRepository;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerSecretRoleResponse>> GetPlayerSecretRoleAsync(PlayerSessionRequest request)
        {
            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Unable to find session");
            }
            if (!session.CurrentRoundId.HasValue)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Session does not have current round");
            }

            var playerRoundInformation = await _playerRoundInformationRepository.SingleOrDefaultAsync(p => p.RoundId == session.CurrentRoundId.Value && p.PlayerId == request.PlayerId);

            if (playerRoundInformation == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Player round information not found");
            }

            var mayor = await _playerRoundInformationRepository.GetMayorAsync(session.CurrentRoundId.Value);
            if (mayor == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Could not find the mayor!");
            }

            return GenericResponse<PlayerSecretRoleResponse>.Ok(new PlayerSecretRoleResponse 
            { 
                SecretRole = (SecretRole)(int)playerRoundInformation.SecretRole,
                MayorName = mayor.Player.Name,
                MayorPlayerId = mayor.PlayerId
            });
        }
    }
}
