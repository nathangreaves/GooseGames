using Entities.Werewords;
using Entities.Werewords.Enums;
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
    public class PlayerRoundInformationService
    {
        private readonly SessionService _sessionService;
        private readonly Global.PlayerService _playerService;
        private readonly IPlayerRoundInformationRepository _playerRoundInformationRepository;
        private readonly RequestLogger<PlayerRoundInformationService> _logger;

        public PlayerRoundInformationService(SessionService sessionService,
            Global.PlayerService playerService,
            IPlayerRoundInformationRepository playerRoundInformationRepository,            
            RequestLogger<PlayerRoundInformationService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _playerRoundInformationRepository = playerRoundInformationRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerSecretRoleResponse>> GetPlayerSecretRoleAsync(PlayerSessionRequest request)
        {
            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Unable to find session");
            }
            if (!session.GameSessionId.HasValue)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Session does not have current round");
            }

            var playerRoundInformation = await _playerRoundInformationRepository.SingleOrDefaultAsync(p => p.RoundId == session.GameSessionId.Value && p.PlayerId == request.PlayerId);

            if (playerRoundInformation == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Player round information not found");
            }

            var mayorPlayerId = await _playerRoundInformationRepository.GetMayorAsync(session.GameSessionId.Value);
            if (mayorPlayerId == null)
            {
                return GenericResponse<PlayerSecretRoleResponse>.Error("Could not find the mayor!");
            }

            var mayorPlayerName = await _playerService.GetPlayerNameAsync(mayorPlayerId.Value);

            var knowledgeOfOtherPlayers = new List<OtherPlayerSecretRoleResponse>();

            var playerReceivesKnowledgeOfWerewolves = PlayerReceivesKnowledgeOfWerewolves(playerRoundInformation);
            if (playerReceivesKnowledgeOfWerewolves)
            {
                var otherWerewolves = await _playerRoundInformationRepository.FilterAsync(p => 
                                                        p.RoundId == playerRoundInformation.RoundId && 
                                                        p.PlayerId != request.PlayerId && 
                                                        p.SecretRole == SecretRolesEnum.Werewolf);

                var playerNames = await _playerService.GetPlayerNamesAsync(otherWerewolves.Select(oW => oW.PlayerId));

                knowledgeOfOtherPlayers.AddRange(otherWerewolves.Select(oW => new OtherPlayerSecretRoleResponse { 
                    PlayerId = oW.PlayerId,
                    SecretRole = (SecretRole)(int)oW.SecretRole,
                    PlayerName = playerNames[oW.PlayerId]
                }));
            }

            return GenericResponse<PlayerSecretRoleResponse>.Ok(new PlayerSecretRoleResponse
            {
                SecretRole = (SecretRole)(int)playerRoundInformation.SecretRole,
                MayorName = mayorPlayerName,
                MayorPlayerId = mayorPlayerId.Value,
                KnowledgeAboutOtherPlayers = knowledgeOfOtherPlayers
            });
        }

        internal bool PlayerReceivesKnowledgeOfWerewolves(PlayerRoundInformation playerRoundInformation)
        {
            return playerRoundInformation.SecretRole == SecretRolesEnum.Werewolf;
        }
    }
}
