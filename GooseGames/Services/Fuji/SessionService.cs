using Entities.Fuji;
using Entities.Global.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Requests;
using Models.Responses;
using Models.Responses.Fuji;
using Models.Responses.Fuji.Hands;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Fuji
{
    public class SessionService
    {
        private readonly Global.SessionService _sessionService;
        private readonly PlayerService _playerService;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerInformationRepository _playerRepository;
        private readonly DeckService _deckService;
        private readonly CardService _cardService;
        private readonly FujiHubContext _fujiHubContext;
        private readonly RequestLogger<SessionService> _logger;

        private const int MinNumberOfPlayersPerSession = 2;
        private const int MaxNumberOfPlayersPerSession = 8;

        public SessionService(Global.SessionService sessionService,
            Global.PlayerService playerService,
            IGameRepository gameRepository,
            IPlayerInformationRepository playerRepository,
            DeckService deckService,
            CardService cardService,
            FujiHubContext fujiHubContext,
            RequestLogger<SessionService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _deckService = deckService;
            _cardService = cardService;
            _fujiHubContext = fujiHubContext;
            _logger = logger;
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

            await _sessionService.StartSessionAsync(request.SessionId);

            var globalPlayers = await _playerService.GetForSessionAsync(request.SessionId);
            var random = new Random();
            var activePlayerId = globalPlayers.Skip(random.Next(0, globalPlayers.Count - 1)).Take(1).First().Id;

            _logger.LogTrace("Inserting game");
            var game = new Game
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId,
                ActivePlayerId = activePlayerId
            };
            await _gameRepository.InsertAsync(game);

            _logger.LogTrace("Inserting game player information");
            var playerInformation = globalPlayers.Select(x => new PlayerInformation
            {
                GameId = game.Id,
                PlayerId = x.Id
            });
            await _playerRepository.InsertRangeAsync(playerInformation);

            _logger.LogTrace("Updating global session to game");
            await _sessionService.SetGameSessionIdentifierAsync(request.SessionId, GameEnum.FujiFlush, game.Id);

            _logger.LogTrace("Sending update to clients");
            await _fujiHubContext.SendStartingSessionAsync(request.SessionId);

            await _deckService.PrepareDeckAsync(game.Id);

            await _fujiHubContext.SendBeginSessionAsync(request.SessionId);

            return GenericResponse<bool>.Ok(true);
        }

        internal async Task<GenericResponse<SessionResponse>> GetAsync(PlayerSessionRequest request)
        {
            var gameId = await _sessionService.GetGameIdAsync(request.SessionId, GameEnum.FujiFlush);
            if (gameId == null)
            {
                return GenericResponse<SessionResponse>.Error("Unable to find session");
            }

            var game = await _gameRepository.GetAsync(gameId.Value);

            var players = await _playerRepository.GetForGameIncludePlayedCardsAsync(game.Id);

            var cards = await _deckService.GetHandCardsForGameAsync(game.Id);

            var cardCombinedValues = _cardService.GetCardCombinedValues(players);

            var playersDictionary = await _playerService.GetPlayersAsync(players.Select(p => p.PlayerId));

            return GenericResponse<SessionResponse>.Ok(new SessionResponse
            { 
                Players = players.OrderBy(p => playersDictionary[p.PlayerId].PlayerNumber).Select(p => {
                    var player = playersDictionary[p.PlayerId];
                    return new Models.Responses.Fuji.Players.Player
                    {
                        Id = p.PlayerId,
                        Name = player.Name,
                        PlayerNumber = player.PlayerNumber,
                        Emoji = player.Emoji,
                        PlayedCard = p.PlayedCard != null ? new Models.Responses.Fuji.Cards.PlayedCard
                        {
                            FaceValue = p.PlayedCard.FaceValue,
                            CombinedValue = cardCombinedValues[p.PlayedCard.FaceValue]
                        } : null,
                        Hand = new ConcealedHand
                        {
                            NumberOfCards = cards.Where(c => c.PlayerId == p.PlayerId && (p.PlayedCardId == null || p.PlayedCardId != c.Id)).Count()
                        },
                        IsActivePlayer = game.ActivePlayerId == p.PlayerId
                    };
                })
            });
        }
    }
}
