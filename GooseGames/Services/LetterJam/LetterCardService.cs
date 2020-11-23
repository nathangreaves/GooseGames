using Entities.LetterJam;
using GooseGames.Logging;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using Models.Responses.LetterJam;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class LetterCardService
    {
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly INonPlayerCharacterRepository _nonPlayerCharacterRepository;
        private readonly RequestLogger<LetterCardService> _requestLogger;

        public LetterCardService(
            ILetterCardRepository letterCardRepository,
            IGameRepository gameRepository,
            IPlayerStateRepository playerStateRepository,
            INonPlayerCharacterRepository nonPlayerCharacterRepository,
            RequestLogger<LetterCardService> requestLogger)
        {
            _letterCardRepository = letterCardRepository;
            _gameRepository = gameRepository;
            _playerStateRepository = playerStateRepository;
            _nonPlayerCharacterRepository = nonPlayerCharacterRepository;
            _requestLogger = requestLogger;
        }


        public static Dictionary<char, int> s_LetterConfiguration = new Dictionary<char, int>
        {
            { 'A', 4 },
            { 'B', 2 },
            { 'C', 3 },
            { 'D', 3 },
            { 'E', 6 },
            { 'F', 2 },
            { 'G', 2 },
            { 'H', 3 },
            { 'I', 4 },
            //{ 'J', 0 },
            { 'K', 2 },
            { 'L', 3 },
            { 'M', 2 },
            { 'N', 3 },
            { 'O', 4 },
            { 'P', 2 },
            //{ 'Q', 0 },
            { 'R', 4 },
            { 'S', 4 },
            { 'T', 4 },
            { 'U', 3 },
            //{ 'V', 0 },
            { 'W', 2 },
            //{ 'X', 0 },
            { 'Y', 2 },
            //{ 'Z', 0 }
        };

        public async Task GenerateDeckForGameAsync(Guid gameId)
        {
            var letterCards = s_LetterConfiguration.SelectMany(c => SpamLetters(c.Key, c.Value)).Select(c => new LetterCard
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                Letter = c
            });

            await _letterCardRepository.InsertRangeAsync(letterCards);
        }

        private static IEnumerable<char> SpamLetters(char c, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return c;
            }
        }

        public async Task<GenericResponse<IEnumerable<LetterCardResponse>>> GetRelevantLettersAsync(PlayerSessionGameRequest request)
        {
            var currentRoundId = await _gameRepository.GetPropertyAsync(request.GameId, g => g.CurrentRoundId);
            if (!currentRoundId.HasValue)
            {
                return GenericResponse<IEnumerable<LetterCardResponse>>.Error("Unable to find current round id");
            }

            var playerCardIds = (await _playerStateRepository.GetPropertyForFilterAsync(p => p.GameId == request.GameId && p.PlayerId != request.PlayerId,
                p => new KeyValuePair<Guid, Guid>(p.Id, p.CurrentLetterId.Value))).Select(p => p.Value).ToList();
            var nonPlayerCardIds = (await _nonPlayerCharacterRepository.GetPropertyForFilterAsync(p => p.GameId == request.GameId,
                p => new KeyValuePair<Guid, Guid>(p.Id, p.CurrentLetterId.Value))).Select(p => p.Value).ToList();

            var cards = await _letterCardRepository.FilterAsync(l => l.GameId == request.GameId && (playerCardIds.Contains(l.Id) || nonPlayerCardIds.Contains(l.Id) || l.BonusLetter));

            return GenericResponse<IEnumerable<LetterCardResponse>>.Ok(cards.OrderBy(c => c.PlayerId.HasValue ? 0 : c.NonPlayerCharacterId.HasValue ? 10 : 20).Select(c => 
            { 
                return new LetterCardResponse 
                {
                    CardId = c.Id,
                    BonusLetter = c.BonusLetter,
                    Letter = c.Letter,
                    PlayerId = c.PlayerId,
                    NonPlayerCharacterId = c.NonPlayerCharacterId
                };
            }));
        }
        internal async Task<GenericResponse<IEnumerable<LetterCardResponse>>> GetLettersAsync(LetterCardsRequest request)
        {
            var listOfCardIds = request.CardIds.ToList();

            var cards = await _letterCardRepository.FilterAsync(l => l.GameId == request.GameId && listOfCardIds.Contains(l.Id));

            return GenericResponse<IEnumerable<LetterCardResponse>>.Ok(cards.Select(c =>
            {
                return new LetterCardResponse
                {
                    CardId = c.Id,
                    BonusLetter = c.BonusLetter,
                    Letter = c.Letter
                };
            }));
        }

    }
}
