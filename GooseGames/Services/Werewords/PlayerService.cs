using Entities.Werewords;
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
    public class PlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly RequestLogger<PlayerService> _logger;

        public PlayerService(IPlayerRepository playerRepository,
            IRoundRepository roundRepository,
            ISessionRepository sessionRepository,
            IPlayerRoundInformationRepository playerRoundInformationRepository,
            RequestLogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _roundRepository = roundRepository;
            _sessionRepository = sessionRepository;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<PlayerSecretRoleResponse>> GetSecretRoleAsync(PlayerSessionRequest request)
        {
            var session = await _sessionRepository.GetAsync(request.SessionId);

            if (session == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Session did not exist");
            }

            if (session.CurrentRoundId == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Session current round did not exist");
            }
            var playerInformation = await _playerRoundInformationRepository.SingleOrDefaultAsync(i => i.PlayerId == request.PlayerId && i.RoundId == session.CurrentRoundId.Value);

            if (playerInformation == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Could not find player information");
            }

            Guid? mayorId = playerInformation.PlayerId;
            if (!playerInformation.IsMayor)
            {
                mayorId = (await _playerRoundInformationRepository.SingleOrDefaultAsync(i => i.RoundId == session.CurrentRoundId.Value && i.IsMayor))?.PlayerId;
            }

            if (mayorId == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Could not find the mayor!");
            }

            var mayorName = await _playerRepository.GetPropertyAsync(mayorId.Value, m => m.Name);

            return GenericResponse<PlayerSecretRoleResponse>.Ok(new PlayerSecretRoleResponse 
            {
                MayorName = mayorName,
                MayorPlayerId = mayorId.Value,
                Role = (SecretRole)(int)playerInformation.SecretRole
            });
        }
    }
}
