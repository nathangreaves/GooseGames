using Entities.LetterJam.Enums;
using GooseGames.Hubs;
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
    public class MyJamService
    {
        private readonly IRoundRepository _roundRepository;
        private readonly IClueRepository _clueRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly IClueLetterRepository _clueLetterRepository;
        private readonly LetterJamHubContext _letterJamHubContext;
        private readonly RequestLogger<MyJamService> _logger;

        public MyJamService(
            IRoundRepository roundRepository,
            IClueRepository clueRepository,
            IPlayerStateRepository playerStateRepository,
            ILetterCardRepository letterCardRepository,
            IClueLetterRepository clueLetterRepository,
            LetterJamHubContext letterJamHubContext,
            RequestLogger<MyJamService> logger)
        {
            _roundRepository = roundRepository;
            _clueRepository = clueRepository;
            _playerStateRepository = playerStateRepository;
            _letterCardRepository = letterCardRepository;
            _clueLetterRepository = clueLetterRepository;
            _letterJamHubContext = letterJamHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponse<MyJamResponse>> GetMyJamAsync(PlayerSessionGameRequest request)
        {
            var rounds = await _roundRepository.FilterAsync(r => r.GameId == request.GameId && r.RoundStatus == RoundStatus.ReceivedClue);

            var player = await _playerStateRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId);

            var letters = await _letterCardRepository.FilterAsync(lC => lC.PlayerId == request.PlayerId);
            var orderedLetters = letters.OrderBy(l => l.LetterIndex.GetValueOrDefault(int.MaxValue));

            var clueLetters = await _clueLetterRepository.GetForCluesAsync(rounds.Select(r => r.ClueId.Value));

            return GenericResponse<MyJamResponse>.Ok(new MyJamResponse
            {
                Rounds = rounds.OrderBy(r => r.RoundNumber).Select(r => {
                    var roundClueLetters = clueLetters[r.ClueId.Value];
                    return new MyJamRound 
                    {
                        ClueGiverPlayerId = r.ClueGiverPlayerId.Value,
                        ClueId = r.ClueId.Value,
                        Letters = roundClueLetters.OrderBy(c => c.LetterIndex).Select(c => new ClueLetterResponse
                        { 
                            CardId = c.LetterCardId,
                            Letter = c.Letter,
                            BonusLetter = c.BonusLetter,
                            IsWildCard = c.IsWildCard,
                            PlayerId = c.PlayerId,
                            NonPlayerCharacterId = c.NonPlayerCharacterId
                        }),
                        RequestingPlayerReceivedClue = roundClueLetters.Any(c => c.PlayerId == request.PlayerId)
                    };
                }),
                NumberOfLetters = player.OriginalWordLength,
                CurrentLetterIndex = player.CurrentLetterIndex,
                MyLetters = orderedLetters.Select(l => {
                    return new MyJamLetterCard 
                    {
                        CardId = l.Id,
                        PlayerLetterGuess = l.PlayerLetterGuess,
                        BonusLetter = l.BonusLetter
                    };
                })
            });
        }

        internal async Task<GenericResponseBase> PostLetterGuessesAsync(MyJamLetterGuessesRequest request)
        {
            var cardList = request.LetterGuesses.Select(x => x.CardId).ToList();

            var cards = await _letterCardRepository.FilterAsync(c => cardList.Contains(c.Id));

            var playerState = await _playerStateRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId);

            foreach (var card in cards)
            {
                var guess = request.LetterGuesses.Single(g => g.CardId == card.Id);
                card.PlayerLetterGuess = guess.PlayerLetterGuess;

                if (card.BonusLetter && guess.PlayerLetterGuess.HasValue)
                {
                    var correct = card.Letter == guess.PlayerLetterGuess;
                    await _letterJamHubContext.SendBonusLetterGuessedAsync(request.SessionId, new Models.HubMessages.LetterJam.BonusLetterGuessMessage 
                    { 
                        ActualLetter = card.Letter,
                        GuessedLetter = guess.PlayerLetterGuess.Value,
                        CardId = card.Id,
                        Correct = correct,
                        PlayerId = request.PlayerId
                    });
                    card.PlayerId = null;
                    //Either way card no longer belongs to player

                    if (card.Letter == guess.PlayerLetterGuess)
                    {
                        //TODO: ?
                    }
                    else
                    {
                        card.BonusLetter = false;
                        card.Discarded = true;
                    }

                    playerState.CurrentLetterId = null;
                    playerState.CurrentLetterIndex = null;
                    await _playerStateRepository.UpdateAsync(playerState);
                }
            }

            if (request.MoveOnToNextLetter)
            {
                var nextCard = cards.Where(c => c.LetterIndex > playerState.CurrentLetterIndex).OrderBy(c => c.LetterIndex).FirstOrDefault();

                if (nextCard != null)
                {
                    playerState.CurrentLetterIndex = nextCard.LetterIndex;
                    playerState.CurrentLetterId = nextCard.Id;

                    await _letterJamHubContext.SendPlayerMovedOnToNextCard(request.SessionId, request.PlayerId, new LetterCardResponse
                    {
                        CardId = nextCard.Id,
                        PlayerId = playerState.PlayerId,
                        BonusLetter = false,
                        Letter = nextCard.Letter,
                        NonPlayerCharacterId = null
                    });
                }
                else
                {
                    playerState.CurrentLetterIndex = null;
                    playerState.CurrentLetterId = null;

                    await _letterJamHubContext.SendPlayerMovedOnToNextCard(request.SessionId, request.PlayerId, null);
                }

                await _playerStateRepository.UpdateAsync(playerState);
            }

            await _letterCardRepository.UpdateRangeAsync(cards);

            return GenericResponseBase.Ok();
        }
    }
}
