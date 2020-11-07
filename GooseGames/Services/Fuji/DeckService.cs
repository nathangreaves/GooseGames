using Entities.Fuji;
using Entities.Fuji.Cards;
using GooseGames.Logging;
using GooseGames.Services.Global;
using Models.Requests.Fuji;
using Models.Responses;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Fuji
{
    public class DeckService
    {
        public static Dictionary<int, int> DeckDistributions = new Dictionary<int, int>
        {
            { 2, 16 },
            { 3, 12 },
            { 4, 9 },
            { 5, 8 },
            { 6, 6 },
            { 7, 6 },
            { 8, 5 },
            { 9, 4 },
            { 10, 4 },
            { 11, 4 },
            { 12, 3 },
            { 13, 3 },
            { 14, 3 },
            { 15, 2 },
            { 16, 1 },
            { 17, 1 },
            { 18, 1 },
            { 19, 1 },
            { 20, 1 }
        };

        private static Dictionary<int, int> s_PlayerCountStartingHandSizes = new Dictionary<int, int>
        {
            { 2, 6 },
            { 3, 6 },
            { 4, 6 },
            { 5, 6 },
            { 6, 6 },
            { 7, 5 },
            { 8, 5 }
        };

        private readonly IPlayerInformationRepository _playerRepository;
        private readonly IHandCardRepository _handCardRepository;
        private readonly IDeckCardRepository _deckCardRepository;
        private readonly IDiscardedCardRepository _discardedCardRepository;
        private readonly RequestLogger<DeckService> _logger;

        private static readonly Random s_Random = new Random();

        public DeckService(
            IPlayerInformationRepository playerRepository, 
            IHandCardRepository handCardRepository, 
            IDeckCardRepository deckCardRepository, 
            IDiscardedCardRepository discardedCardRepository, 
            RequestLogger<DeckService> logger)
        {
            _playerRepository = playerRepository;
            _handCardRepository = handCardRepository;
            _deckCardRepository = deckCardRepository;
            _discardedCardRepository = discardedCardRepository;
            _logger = logger;
        }

        internal async Task<GenericResponseBase> PrepareDeckAsync(Guid gameId)
        {
            List<DeckCard> deckCards = CreateNewDeck(gameId);

            var players = await _playerRepository.FilterAsync(p => p.GameId == gameId);

            var cardsPerPlayer = s_PlayerCountStartingHandSizes[players.Count];
            foreach (var player in players)
            {
                List<HandCard> handCard = new List<HandCard>();
                for (int i = 0; i < cardsPerPlayer; i++)
                {
                    int deckIndex = 0;                    
                   
                    var card = deckCards[deckIndex];
                    deckCards.RemoveAt(deckIndex);

                    handCard.Add(DealToPlayer(card, player));
                }

                await _handCardRepository.InsertRangeAsync(handCard);
            }

            await _deckCardRepository.InsertRangeAsync(deckCards);

            return GenericResponseBase.Ok();
        }

        public static List<DeckCard> CreateNewDeck(Guid gameId)
        {
            var list = new List<int> { };

            foreach (var cardValue in DeckDistributions)
            {
                for (int i = 0; i < cardValue.Value; i++)
                {
                    InsertIntoListAtRandomPosition(list, cardValue.Key);
                }
            }

            List<DeckCard> deckCards = CreateDeck(gameId, list);
            return deckCards;
        }

        private static void InsertIntoListAtRandomPosition<T>(List<T> list, T cardValue)
        {
            list.Insert(s_Random.Next(0, list.Count), cardValue);
        }

        private static List<DeckCard> CreateDeck(Guid gameId, List<int> list)
        {
            int o = 1;

            var createdDate = DateTime.UtcNow;
            var deckCards = list.Select(l => new DeckCard
            {
                FaceValue = l,
                Order = o++,
                GameId = gameId,
                CreatedUtc = createdDate
            }).ToList();
            return deckCards;
        }

        internal async Task DiscardCardAsync(HandCard card)
        {
            await _handCardRepository.DeleteAsync(card);

            await _discardedCardRepository.InsertAsync(new DiscardedCard 
            {
                Id = card.Id,
                FaceValue = card.FaceValue,
                GameId = card.GameId
            });
        }

        private HandCard DealToPlayer(DeckCard deckCard, PlayerInformation player)
        {
            return new HandCard
            {
                Id = deckCard.Id,
                FaceValue = deckCard.FaceValue,
                PlayerId = player.PlayerId,
                GameId = player.GameId,
                CreatedUtc = DateTime.UtcNow
            };
        }

        internal async Task<List<HandCard>> GetHandCardsForGameAsync(Guid gameId)
        {
            return await _handCardRepository.FilterAsync(c => c.GameId == gameId);
        }

        internal async Task<HandCard> DealNewCardToPlayerAsync(PlayerInformation player)
        {
            var deckCard = await _deckCardRepository.GetNextCardAsync(player.GameId);

            if (deckCard == null)
            {
                //Shuffle discards into deck
                await ShuffleDeckAsync(player.GameId);

                deckCard = await _deckCardRepository.GetNextCardAsync(player.GameId);
            }

            var handCard = DealToPlayer(deckCard, player);

            await _deckCardRepository.DeleteAsync(deckCard);
            await _handCardRepository.InsertAsync(handCard);

            return handCard;
        }

        private async Task ShuffleDeckAsync(Guid gameId)
        {
            var discardedCards = await _discardedCardRepository.FilterAsync(c => c.GameId == gameId);

            List<int> cardValues = new List<int>();
            foreach (var item in discardedCards.Select(c => c.FaceValue))
            {
                InsertIntoListAtRandomPosition(cardValues, item);
            }

            var deck = CreateDeck(gameId, cardValues);

            await _deckCardRepository.InsertRangeAsync(deck);
        }
    }
}
