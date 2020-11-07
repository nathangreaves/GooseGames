using Entities.Fuji;
using Entities.Fuji.Cards;
using Entities.Global.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.EntityFrameworkCore.Metadata;
using Models.HubMessages.Fuji;
using Models.Requests.Fuji;
using Models.Responses;
using Models.Responses.Fuji.Cards;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace GooseGames.Services.Fuji
{
    public class CardService
    {
        private readonly Global.SessionService _sessionService;
        private readonly Global.PlayerService _playerService;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerInformationRepository _playerRepository;
        private readonly IHandCardRepository _handCardRepository;
        private readonly DeckService _deckService;
        private readonly FujiHubContext _fujiHubContext;
        private readonly RequestLogger<CardService> _logger;

        public CardService(Global.SessionService sessionService, 
            Global.PlayerService playerService,
            IGameRepository gameRepository,
            IPlayerInformationRepository playerRepository,
            IHandCardRepository handCardRepository,
            DeckService deckService,
            FujiHubContext fujiHubContext,
            RequestLogger<CardService> logger)
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _handCardRepository = handCardRepository;
            _deckService = deckService;
            _fujiHubContext = fujiHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponse<Card>> GetHandCardAsync(CardRequest request)
        {
            var card = await _handCardRepository.GetAsync(request.CardId);
            if (card == null)
            {
                return GenericResponse<Card>.Error("Card not found");
            }

            return GenericResponse<Card>.Ok(new Card 
            {
                Id = card.Id,
                FaceValue = card.FaceValue
            });
        }

        internal async Task<GenericResponseBase> PlayCardAsync(PlayCardRequest request)
        {
            var gameId = await _sessionService.GetGameIdAsync(request.SessionId, GameEnum.FujiFlush);
            if (gameId == null)
            {
                return GenericResponseBase.Error("Unable to find session");
            }

            var game = await _gameRepository.GetAsync(gameId.Value);
            if (game == null)
            {
                return GenericResponseBase.Error("Could not get session");
            }

            var players = await _playerRepository.GetForGameIncludePlayedCardsAsync(game.Id);
            if (players == null)
            {
                return GenericResponseBase.Error("Could not get players for session");
            }

            var currentPlayer = players.SingleOrDefault(p => p.PlayerId == request.PlayerId);
            if (currentPlayer == null)
            {
                return GenericResponseBase.Error("Could not get player");
            }

            var playedCard = await _handCardRepository.GetAsync(request.CardId);
            if (playedCard == null)
            {
                return GenericResponseBase.Error("Could not get card");
            }
            if (playedCard.PlayerId != currentPlayer.PlayerId)
            {
                return GenericResponseBase.Error("Card did not match player");
            }

            currentPlayer.PlayedCard = playedCard;
            await _playerRepository.UpdateAsync(currentPlayer);
            FujiUpdate fujiUpdate = await UpdatePlayersAsync(players, currentPlayer, playedCard);

            var activePlayerId = await GetNextActivePlayerAsync(request.SessionId, currentPlayer.PlayerId);
            game.ActivePlayerId = activePlayerId;

            var activePlayerInfo = await _playerRepository.GetPlayerInformationFromPlayerIdAndGameId(activePlayerId, gameId.Value);

            var activePlayerUpdate = new ActivePlayerUpdate
            {
                ActivePlayerId = activePlayerId,
                DiscardedCards = new List<PlayerDiscardUpdate>()
            };
            var gameVictoryUpdate = new GameVictoryUpdate
            {
                WinningPlayers = new List<Guid>()
            };

            if (activePlayerInfo.PlayedCard != null)
            {
                var faceValue = activePlayerInfo.PlayedCard.FaceValue;
                foreach (var player in players.Where(p => p.PlayedCard != null && p.PlayedCard.FaceValue == faceValue))
                {
                    await _deckService.DiscardCardAsync(player.PlayedCard);

                    player.PlayedCardId = null;
                    await _playerRepository.UpdateAsync(player);

                    var playerCards = await _handCardRepository.CountAsync(c => c.PlayerId == player.PlayerId && c.GameId == game.Id);

                    activePlayerUpdate.DiscardedCards.Add(new PlayerDiscardUpdate 
                    {
                        PlayerId = player.PlayerId
                    });

                    if (playerCards == 0)
                    {
                        gameVictoryUpdate.WinningPlayers.Add(player.PlayerId);
                    }
                }
            }

            fujiUpdate.ActivePlayerUpdate = activePlayerUpdate;
            if (gameVictoryUpdate.WinningPlayers.Any())
            {
                fujiUpdate.GameVictoryUpdate = gameVictoryUpdate;
                await UpdateSessionToCompleteAsync(game.SessionId);
            }
            await _gameRepository.UpdateAsync(game);

            await _fujiHubContext.SendUpdateSessionAsync(game.SessionId, fujiUpdate);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> DiscardHandCardAsync(Guid cardId)
        {
            var card = await _handCardRepository.GetAsync(cardId);
            if (card == null)
            {
                return GenericResponseBase.Error("Unable to find card");
            }

            var player = await _playerRepository.GetPlayerInformationFromPlayerIdAndGameId(card.PlayerId, card.GameId);
            if (player == null)
            {
                return GenericResponseBase.Error("Unable to find card player");
            }

            await _deckService.DiscardCardAsync(card);

            player.PlayedCardId = null;
            await _playerRepository.UpdateAsync(player);

            var playerCards = await _handCardRepository.CountAsync(c => c.PlayerId == card.PlayerId && c.GameId == card.GameId);

            if (playerCards == 0)
            {
                var game = await _gameRepository.GetAsync(player.GameId);

                //Player has won
                await _fujiHubContext.SendPlayerVictoryAsync(game.SessionId, card.PlayerId);

                await UpdateSessionToCompleteAsync(game.SessionId);
            }

            return GenericResponseBase.Ok();
        }

        private async Task UpdateSessionToCompleteAsync(Guid sessionId)
        {            
            await _sessionService.SetToLobbyAsync(sessionId);
        }

        private async Task<FujiUpdate> UpdatePlayersAsync(List<PlayerInformation> players, PlayerInformation currentPlayer, HandCard playedCard)
        {
            var combinedValues = GetCardCombinedValues(players);

            var currentPlayerFaceValue = playedCard.FaceValue;
            var currentPlayerCombinedCardValue = combinedValues[currentPlayerFaceValue];

            var fujiUpdate = new FujiUpdate
            {
                PlayedCards = new List<PlayedCardUpdate>
                {
                    new PlayedCardUpdate
                    {
                        PlayerId = currentPlayer.PlayerId,
                        FaceValue = currentPlayerFaceValue,
                        CombinedValue = currentPlayerCombinedCardValue
                    }
                },
                DiscardedCards = new List<PlayerDiscardUpdate>(),
                NewDraws = new List<PlayerDrawUpdate>()
            };

            foreach (var player in players.Where(p => p.PlayedCard != null && p.PlayerId != currentPlayer.PlayerId))
            {
                var playerFaceValue = player.PlayedCard.FaceValue;
                var playerCombinedValue = combinedValues[playerFaceValue];

                if (playerFaceValue == currentPlayerFaceValue)
                {
                    //This player's combined value has increased!
                    fujiUpdate.PlayedCards.Add(new PlayedCardUpdate
                    {
                        PlayerId = player.PlayerId,
                        FaceValue = currentPlayerFaceValue,
                        CombinedValue = currentPlayerCombinedCardValue
                    });
                }
                else if (playerCombinedValue < currentPlayerCombinedCardValue)
                {
                    //This player must discard and redraw!
                    fujiUpdate.DiscardedCards.Add(new PlayerDiscardUpdate
                    {
                        PlayerId = player.PlayerId
                    });
                    await _deckService.DiscardCardAsync(player.PlayedCard);

                    var newCard = await _deckService.DealNewCardToPlayerAsync(player);
                    fujiUpdate.NewDraws.Add(new PlayerDrawUpdate
                    {
                        PlayerId = player.PlayerId,
                        NewCardId = newCard.Id
                    });

                    player.PlayedCardId = null;
                    await _playerRepository.UpdateAsync(player);
                }
                else
                {
                    //Player has a higher card so is unaffected
                }
            }

            return fujiUpdate;
        }

        internal Dictionary<int, int> GetCardCombinedValues(IEnumerable<PlayerInformation> players)
        {
            return players.Select(p => p.PlayedCard).Where(p => p != null).GroupBy(c => c.FaceValue).ToDictionary(c => c.Key, c => c.Key * c.Count());
        }

        private async Task<Guid> GetNextActivePlayerAsync(Guid sessionId, Guid? previousActivePlayerId)
        {
            return await _playerService.GetNextActivePlayerAsync(sessionId, previousActivePlayerId);
        }
    }
}
