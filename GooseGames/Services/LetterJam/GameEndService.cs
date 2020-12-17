using Models.Requests;
using Models.Responses;
using Models.Responses.LetterJam;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class GameEndService
    {
        private readonly IFinalWordLetterRepository _finalWordLetterRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly IGameRepository _gameRepository;

        public GameEndService(IFinalWordLetterRepository finalWordLetterRepository,
            ILetterCardRepository letterCardRepository,
            IGameRepository gameRepository)
        {
            _finalWordLetterRepository = finalWordLetterRepository;
            _letterCardRepository = letterCardRepository;
            _gameRepository = gameRepository;
        }

        internal async Task<GenericResponse<GameEndResponse>> GetGameEndAsync(PlayerSessionGameRequest request)
        {
            var game = await _gameRepository.GetAsync(request.GameId);

            var allFinalWordLetters = await _finalWordLetterRepository.FilterAsync(l => l.GameId == request.GameId);
            var allPlayerLetters = await _letterCardRepository.FilterAsync(l => l.GameId == request.GameId && l.PlayerId.HasValue);
            var groupedFinalWordLetters = allFinalWordLetters.GroupBy(f => f.PlayerId).ToDictionary(g => g.Key, g => g);
            var groupedPlayerLetters = allPlayerLetters.GroupBy(f => f.PlayerId).ToDictionary(g => g.Key.Value, g => g);

            var list = new List<GameEndPlayer>();

            foreach (var playerId in groupedPlayerLetters.Keys)
            {
                var letters = groupedPlayerLetters[playerId];
                var finalWordLetters = groupedFinalWordLetters[playerId];                

                var lettersNotInFinalWord = letters.Where(l => !finalWordLetters.Any(f => f.CardId == l.Id));

                list.Add(new GameEndPlayer
                {
                    PlayerId = playerId,
                    FinalWordLetters = finalWordLetters.OrderBy(f => f.LetterIndex).Select(f =>
                    {
                        return new GameEndPlayerLetter
                        {
                            CardId = f.CardId,
                            BonusLetter = f.BonusLetter,
                            IsWildCard = f.Wildcard,
                            PlayerLetterGuess = f.PlayerLetterGuess,
                            Letter = f.Letter,
                            LetterIndex = letters.FirstOrDefault(l => f.CardId.HasValue && l.Id == f.CardId)?.LetterIndex
                        };
                    }),
                    OriginalWordLetters = letters.OrderBy(l => l.OriginalLetterIndex).Select(f => {

                        return new GameEndPlayerLetter
                        {
                            CardId = f.Id,
                            BonusLetter = f.BonusLetter,
                            IsWildCard = false,
                            PlayerLetterGuess = f.PlayerLetterGuess,
                            Letter = f.Letter,
                            LetterIndex = f.LetterIndex
                        };
                    }),
                    UnusedLetters = lettersNotInFinalWord.Select(l => {
                        return new GameEndPlayerLetter
                        {
                            CardId = l.Id,
                            BonusLetter = false,
                            IsWildCard = false,
                            PlayerLetterGuess = l.PlayerLetterGuess,
                            Letter = l.Letter,
                            LetterIndex = l.LetterIndex
                        };
                    })
                });
            }

            return GenericResponse<GameEndResponse>.Ok(new GameEndResponse
            {
                Players = list,
                CluesRemaining = game.GreenCluesRemaining + game.RedCluesRemaining
            });
        }
    }
}
