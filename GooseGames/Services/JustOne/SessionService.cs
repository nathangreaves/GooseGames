using Entities.Global.Enums;
using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Microsoft.AspNetCore.SignalR;
using Models.Requests;
using Models.Requests.JustOne;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.PlayerDetails;
using Models.Responses.Sessions;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GooseGames.Services.JustOne
{
    public class SessionService
    {
        private readonly Global.SessionService _sessionService;
        private readonly PlayerService _playerService;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly PlayerStatusService _playerStatusService;
        private readonly RoundService _roundService;
        private readonly RequestLogger<SessionService> _logger;
        private readonly JustOneHubContext _lobbyHub;
        private const int MinNumberOfPlayersPerSession = 3;
        private const int MaxNumberOfPlayersPerSession = 7;

        public SessionService(Global.SessionService sessionService, 
            Global.PlayerService playerService, 
            IGameRepository gameRepository,
            IPlayerStatusRepository playerStatusRepository,
            PlayerStatusService playerStatusService, 
            RoundService roundService,
            RequestLogger<SessionService> logger,
            JustOneHubContext lobbyHub)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _gameRepository = gameRepository;
            _playerStatusRepository = playerStatusRepository;
            _playerStatusService = playerStatusService;
            _roundService = roundService;
            _logger = logger;
            _lobbyHub = lobbyHub;
        }

        internal async Task<GenericResponseBase> StartSessionAsync(StartSessionRequest request)
        {
            _logger.LogTrace("Starting session", request);
            var validationResponse = await _sessionService.ValidateSessionToStartAsync(request, MinNumberOfPlayersPerSession, MaxNumberOfPlayersPerSession);
            if (!validationResponse.Success)
            {
                return validationResponse;
            }
            _logger.LogTrace("Session cleared to start");

            await _sessionService.StartSessionAsync(request.SessionId);

            _logger.LogTrace("Fetching players");
            var globalPlayers = await _playerService.GetForSessionAsync(request.SessionId);


            _logger.LogTrace("Inserting game");
            var game = new Game
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId                
            };
            await _gameRepository.InsertAsync(game);

            _logger.LogTrace("Inserting game player information");
            var playerInformation = globalPlayers.Select(x => new PlayerStatus
            {
                GameId = game.Id,
                PlayerId = x.Id,
                Status = Entities.JustOne.Enums.PlayerStatusEnum.RoundWaiting
            });
            await _playerStatusRepository.InsertRangeAsync(playerInformation);

            _logger.LogTrace("Updating global session to game");
            await _sessionService.SetGameSessionIdentifierAsync(request.SessionId, GameEnum.JustOne, game.Id);

            _logger.LogTrace("Sending update to clients");
            await _lobbyHub.SendStartingSessionAsync(request.SessionId);
            
            await _roundService.PrepareRoundsAsync(game.Id, request.IncludedWordLists);

            return GenericResponse<bool>.Ok(true);
        }
    }
}
