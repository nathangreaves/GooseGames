using Entities.Fuji;
using Entities.Fuji.Cards;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.HubMessages.Fuji;
using Models.Requests.Fuji;
using Models.Responses;
using Models.Responses.Fuji.Cards;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Fuji
{
    public class CardService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IHandCardRepository _handCardRepository;
        private readonly DeckService _deckService;
        private readonly FujiHubContext _fujiHubContext;
        private readonly RequestLogger<CardService> _logger;

        public CardService(ISessionRepository sessionRepository,
            IPlayerRepository playerRepository,
            IHandCardRepository handCardRepository,
            DeckService deckService,
            FujiHubContext fujiHubContext,
            RequestLogger<CardService> logger)
        {
            _sessionRepository = sessionRepository;
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
            var session = await _sessionRepository.GetAsync(request.SessionId);
            if (session == null)
            {

            }

            var players = await _playerRepository.GetForSessionIncludePlayedCardsAsync(request.SessionId);
            if (players != null)
            {

            }

            var currentPlayer = players.SingleOrDefault(p => p.Id == request.PlayerId);
            if (currentPlayer == null)
            {

            }

            var playedCard = await _handCardRepository.GetAsync(request.CardId);
            if (playedCard == null)
            {

            }
            if (playedCard.PlayerId != currentPlayer.Id)
            {

            }

            currentPlayer.PlayedCard = playedCard;
            await _playerRepository.UpdateAsync(currentPlayer);
            FujiUpdate fujiUpdate = await UpdatePlayersAsync(players, currentPlayer, playedCard);

            var activePlayer = await GetNextActivePlayerAsync(request.SessionId, currentPlayer.Id);
            session.ActivePlayerId = activePlayer.Id;
            await _sessionRepository.UpdateAsync(session);
            fujiUpdate.ActivePlayerUpdate = new ActivePlayerUpdate
            {
                ActivePlayerId = activePlayer.Id
            };

            await _fujiHubContext.UpdateSessionAsync(session.Id, fujiUpdate);

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> DiscardHandCardAsync(Guid cardId)
        {
            var card = await _handCardRepository.GetAsync(cardId);
            if (card == null)
            {
                return GenericResponseBase.Error("Unable to find card");
            }

            var player = await _playerRepository.GetAsync(card.PlayerId);
            if (player == null)
            {
                return GenericResponseBase.Error("Unable to find card player");
            }

            await _deckService.DiscardCardAsync(card);

            player.PlayedCardId = null;
            await _playerRepository.UpdateAsync(player);

            var playerCards = await _handCardRepository.CountAsync(c => c.PlayerId == card.PlayerId);

            if (playerCards == 0)
            {
                //Player has won
                await _fujiHubContext.SendPlayerVictoryAsync(card.SessionId, card.PlayerId);

                var session = await _sessionRepository.GetAsync(card.SessionId);
                session.StatusId = Entities.Fuji.Enums.SessionStatusEnum.Complete;
                await _sessionRepository.UpdateAsync(session);
            }

            return GenericResponseBase.Ok();
        }

        private async Task<FujiUpdate> UpdatePlayersAsync(List<Player> players, Player currentPlayer, HandCard playedCard)
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
                        PlayerId = currentPlayer.Id,
                        FaceValue = currentPlayerFaceValue,
                        CombinedValue = currentPlayerCombinedCardValue
                    }
                },
                DicardedCards = new List<PlayerDiscardUpdate>(),
                NewDraws = new List<PlayerDrawUpdate>()
            };

            foreach (var player in players.Where(p => p.PlayedCard != null && p.Id != currentPlayer.Id))
            {
                var playerFaceValue = player.PlayedCard.FaceValue;
                var playerCombinedValue = combinedValues[playerFaceValue];

                if (playerFaceValue == currentPlayerFaceValue)
                {
                    //This player's combined value has increased!
                    fujiUpdate.PlayedCards.Add(new PlayedCardUpdate
                    {
                        PlayerId = player.Id,
                        FaceValue = currentPlayerFaceValue,
                        CombinedValue = currentPlayerCombinedCardValue
                    });
                }
                else if (playerCombinedValue < currentPlayerCombinedCardValue)
                {
                    //This player must discard and redraw!
                    fujiUpdate.DicardedCards.Add(new PlayerDiscardUpdate
                    {
                        PlayerId = player.Id
                    });
                    await _deckService.DiscardCardAsync(player.PlayedCard);

                    var newCard = await _deckService.DealNewCardToPlayerAsync(player);
                    fujiUpdate.NewDraws.Add(new PlayerDrawUpdate
                    {
                        PlayerId = player.Id,
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

        internal Dictionary<int, int> GetCardCombinedValues(IEnumerable<Player> players)
        {
            return players.Select(p => p.PlayedCard).Where(p => p != null).GroupBy(c => c.FaceValue).ToDictionary(c => c.Key, c => c.Key * c.Count());
        }

        private async Task<Player> GetNextActivePlayerAsync(Guid sessionId, Guid? previousActivePlayerId)
        {
            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId);

            Player previousActivePlayer = null;
            if (previousActivePlayerId == null)
            {
                previousActivePlayer = players[new Random().Next(players.Count)];
            }
            else
            {
                previousActivePlayer = players.Single(p => p.Id == previousActivePlayerId.Value);
            }

            var orderedList = players.OrderBy(x => x.PlayerNumber).ToList();

            if (orderedList.Last().Id == previousActivePlayer.Id)
            {
                return orderedList.First();
            }
            var indexOfPrevious = orderedList.IndexOf(previousActivePlayer);
            return orderedList[indexOfPrevious + 1];
        }
    }
}
