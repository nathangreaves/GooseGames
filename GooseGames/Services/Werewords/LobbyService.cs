using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Requests;
using Models.Requests.PlayerDetails;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.PlayerDetails;
using Models.Responses.Sessions;
using RepositoryInterface.Global;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GooseGames.Services.Werewords
{
    public class LobbyService
    {
        private readonly SessionService _sessionService;
        private readonly Global.PlayerService _playerService;
        private readonly RoundService _roundService;
        private readonly RequestLogger<LobbyService> _logger;
        private readonly WerewordsHubContext _werewordsHubContext;
        private const int MinNumberOfPlayersPerSession = 4;
        private const int MaxNumberOfPlayersPerSession = 10;

        public LobbyService(
            SessionService sessionService,
            Global.PlayerService playerService,
            RoundService roundService,
            RequestLogger<LobbyService> logger,
            WerewordsHubContext lobbyHub)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _roundService = roundService;
            _logger = logger;
            _werewordsHubContext = lobbyHub;
        }

        internal async Task<GenericResponseBase> StartSessionAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Starting session", request);
            var validationResponse = await _sessionService.ValidateSessionToStartAsync(request, MinNumberOfPlayersPerSession, MaxNumberOfPlayersPerSession);
            if (!validationResponse.Success)
            {
                return validationResponse;
            }
            _logger.LogTrace("Session cleared to start");

            _logger.LogTrace("Removing unready players");
            await _sessionService.StartSessionAsync(request.SessionId);
          
            _logger.LogTrace("Sending update to clients");
            await _werewordsHubContext.SendStartingSessionAsync(request.SessionId);

            await Task.Delay(TimeSpan.FromSeconds(2));

            await _roundService.CreateNewRoundAsync(request.SessionId);

            await _sessionService.UpdateAllPlayersToStatusAsync(request.SessionId, Entities.Global.Enums.PlayerStatusEnum.InGame);

            return GenericResponse<bool>.Ok(true);
        }
    }
}
