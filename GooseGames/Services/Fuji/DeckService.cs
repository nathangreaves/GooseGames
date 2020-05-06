using Entities.Fuji;
using Entities.Fuji.Cards;
using GooseGames.Logging;
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

        private readonly IPlayerRepository _playerRepository;
        private readonly IHandCardRepository _handCardRepository;
        private readonly IDeckCardRepository _deckCardRepository;
        private readonly IDiscardedCardRepository _discardedCardRepository;
        private readonly RequestLogger<DeckService> _logger;

        private static readonly Random s_Random = new Random();

        public DeckService(IPlayerRepository playerRepository, IHandCardRepository handCardRepository, IDeckCardRepository deckCardRepository, IDiscardedCardRepository discardedCardRepository, RequestLogger<DeckService> logger)
        {
            _playerRepository = playerRepository;
            _handCardRepository = handCardRepository;
            _deckCardRepository = deckCardRepository;
            _discardedCardRepository = discardedCardRepository;
            _logger = logger;
        }

        internal async Task<GenericResponseBase> PrepareDeckAsync(Guid sessionId, bool testSession = false)
        {
            List<DeckCard> deckCards = CreateNewDeck(sessionId);

            var players = await _playerRepository.FilterAsync(p => p.SessionId == sessionId);

            var cardsPerPlayer = s_PlayerCountStartingHandSizes[players.Count];
            foreach (var player in players)
            {
                List<HandCard> handCard = new List<HandCard>();
                for (int i = 0; i < cardsPerPlayer; i++)
                {
                    int deckIndex = 0;                    
                    if (testSession)
                    {
                        //In a test session, deal everyone at least 1 copy of 2 & 3
                        if (i == 0)
                        {
                            deckIndex = deckCards.FindIndex(0, x => x.FaceValue == 2);
                        }
                        else if (i == 1)
                        {
                            deckIndex = deckCards.FindIndex(0, x => x.FaceValue == 3);
                        }
                    }

                    var card = deckCards[deckIndex];
                    deckCards.RemoveAt(deckIndex);

                    handCard.Add(DealToPlayer(card, player));
                }

                await _handCardRepository.InsertRangeAsync(handCard);
            }

            await _deckCardRepository.InsertRangeAsync(deckCards);

            return GenericResponseBase.Ok();
        }

        public static List<DeckCard> CreateNewDeck(Guid sessionId)
        {
            var list = new List<int> { };

            foreach (var cardValue in DeckDistributions)
            {
                for (int i = 0; i < cardValue.Value; i++)
                {
                    InsertIntoListAtRandomPosition(list, cardValue.Key);
                }
            }

            List<DeckCard> deckCards = CreateDeck(sessionId, list);
            return deckCards;
        }

        private static void InsertIntoListAtRandomPosition<T>(List<T> list, T cardValue)
        {
            list.Insert(s_Random.Next(0, list.Count), cardValue);
        }

        private static List<DeckCard> CreateDeck(Guid sessionId, List<int> list)
        {
            int o = 1;

            var createdDate = DateTime.UtcNow;
            var deckCards = list.Select(l => new DeckCard
            {
                FaceValue = l,
                Order = o++,
                SessionId = sessionId,
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
                SessionId = card.SessionId
            });
        }

        private HandCard DealToPlayer(DeckCard deckCard, Player player)
        {
            return new HandCard
            {
                Id = deckCard.Id,
                FaceValue = deckCard.FaceValue,
                PlayerId = player.Id,
                SessionId = player.SessionId,
                CreatedUtc = DateTime.UtcNow
            };
        }

        internal async Task<List<HandCard>> GetHandCardsForSessionAsync(Guid sessionId)
        {
            return await _handCardRepository.FilterAsync(c => c.SessionId == sessionId);
        }

        internal async Task<HandCard> DealNewCardToPlayerAsync(Player player)
        {
            var deckCard = await _deckCardRepository.GetNextCardAsync(player.SessionId);

            if (deckCard == null)
            {
                //Shuffle discards into deck
                await ShuffleDeckAsync(player.SessionId);

                deckCard = await _deckCardRepository.GetNextCardAsync(player.SessionId);
            }

            var handCard = DealToPlayer(deckCard, player);

            await _deckCardRepository.DeleteAsync(deckCard);
            await _handCardRepository.InsertAsync(handCard);

            return handCard;
        }

        private async Task ShuffleDeckAsync(Guid sessionId)
        {
            var discardedCards = await _discardedCardRepository.FilterAsync(c => c.SessionId == sessionId);

            List<int> cardValues = new List<int>();
            foreach (var item in discardedCards.Select(c => c.FaceValue))
            {
                InsertIntoListAtRandomPosition(cardValues, item);
            }

            var deck = CreateDeck(sessionId, cardValues);

            await _deckCardRepository.InsertRangeAsync(deck);
        }
    }
}
