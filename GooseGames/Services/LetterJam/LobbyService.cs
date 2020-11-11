using Entities.LetterJam;
using Entities.LetterJam.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class LobbyService
    {
        private readonly SessionService _sessionService;
        private readonly PlayerService _playerService;
        private readonly LetterCardService _letterCardService;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly LetterJamHubContext _letterJamHubContext;
        private readonly RequestLogger<LobbyService> _logger;

        private const int MinNumberOfPlayersPerSession = 2;
        private const int MaxNumberOfPlayersPerSession = 6;

        public LobbyService(
            SessionService sessionService,
            PlayerService playerService,
            LetterCardService letterCardService,
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            LetterJamHubContext letterJamHubContext,
            RequestLogger<LobbyService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _letterCardService = letterCardService;
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _letterJamHubContext = letterJamHubContext;
            _logger = logger;
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

            _logger.LogTrace("Removing unready players");
            await _sessionService.StartSessionAsync(request.SessionId);

            await _sessionService.UpdateAllPlayersToStatusAsync(request.SessionId, Entities.Global.Enums.PlayerStatusEnum.InGame);

            _logger.LogTrace("Sending update to clients");
            await _letterJamHubContext.SendStartingSessionAsync(request.SessionId);

            var players = await _playerService.GetForSessionAsync(request.SessionId);

            var numberOfPlayers = players.Count;

            var gameConfiguration = GameConfigurationService.GetForPlayerCount(numberOfPlayers);

            var game = new Game 
            { 
                Id = Guid.NewGuid(),
                GameStatus = Entities.LetterJam.Enums.GameStatus.PreparingStartingWords,
                SessionId = request.SessionId,
                GreenCluesRemaining = gameConfiguration.NumberOfGreenClues,
                RedCluesRemaining = gameConfiguration.NumberOfRedClues,
                LockedCluesRemaining = gameConfiguration.NumberOfLockedGreenClues                
            };
            await _gameRepository.InsertAsync(game);

            var playerStates = players.Select(p => new PlayerState 
            {            
                Id = Guid.NewGuid(),
                GameId = game.Id,
                OriginalWordLength = request.NumberOfLetters,
                PlayerId = p.Id,
                Status = PlayerStatus.ConstructingWord
            });
            await _playerStateRepository.InsertRangeAsync(playerStates);

            await _letterCardService.GenerateDeckForGameAsync(game.Id);

            _logger.LogTrace("Sending update to clients");
            await _letterJamHubContext.SendBeginSessionAsync(request.SessionId, game.Id);

            return GenericResponseBase.Ok();
        }
    }
}
