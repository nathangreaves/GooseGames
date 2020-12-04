using Entities.LetterJam.Enums;
using GooseGames.Logging;
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
    public class MyJamService
    {
        private readonly IRoundRepository _roundRepository;
        private readonly IClueRepository _clueRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly RequestLogger<MyJamService> _logger;

        public MyJamService(
            IRoundRepository roundRepository,
            IClueRepository clueRepository,
            IPlayerStateRepository playerStateRepository,
            ILetterCardRepository letterCardRepository,
            RequestLogger<MyJamService> logger)
        {
            _roundRepository = roundRepository;
            _clueRepository = clueRepository;
            _playerStateRepository = playerStateRepository;
            _letterCardRepository = letterCardRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<MyJamResponse>> GetMyJamAsync(PlayerSessionGameRequest request)
        {
            var rounds = await _roundRepository.FilterAsync(r => r.GameId == request.GameId && r.RoundStatus == RoundStatus.ReceivedClue);

            var player = await _playerStateRepository.SingleOrDefaultAsync(p => p.PlayerId == request.PlayerId);

            var letters = await _letterCardRepository.FilterAsync(lC => lC.PlayerId == request.PlayerId);
            var orderedLetters = letters.OrderBy(l => l.LetterIndex.GetValueOrDefault(int.MaxValue));

            return GenericResponse<MyJamResponse>.Ok(new MyJamResponse
            {
                Rounds = rounds.Select(r => {
                    return new MyJamRound 
                    {
                        ClueGiverPlayerId = r.ClueGiverPlayerId.Value,
                        ClueId = r.ClueId.Value
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
    }
}
