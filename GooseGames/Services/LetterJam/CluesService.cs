using Entities.LetterJam;
using GooseGames.Logging;
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
    public class CluesService
    {
        private readonly GameService _gameService;
        private readonly IClueRepository _clueRepository;
        private readonly IClueVoteRepository _clueVoteRepository;
        private readonly RequestLogger<CluesService> _logger;

        public CluesService(
            GameService gameService,
            IClueRepository clueRepository,
            IClueVoteRepository clueVoteRepository,
            RequestLogger<CluesService> logger)
        {
            _gameService = gameService;
            _clueRepository = clueRepository;
            _clueVoteRepository = clueVoteRepository;
            _logger = logger;
        }

        internal async Task<GenericResponse<IEnumerable<ProposedClueResponse>>> GetProposedCluesAsync(RoundRequest request)
        {
            var nullableRoundId = request.RoundId ?? await _gameService.GetRoundIdAsync(request);
            if (!nullableRoundId.HasValue)
            {
                return GenericResponse<IEnumerable<ProposedClueResponse>>.Error("Unable to get roundId");
            }
            var roundId = nullableRoundId.Value;

            var clues = await _clueRepository.FilterAsync(c => c.RoundId == roundId);
            var clueVotes = await _clueVoteRepository.FilterAsync(cV => cV.RoundId == roundId);

            var clueVotesGrouped = clueVotes.GroupBy(c => c.ClueId).ToDictionary(c => c.Key, c => c);

            return GenericResponse<IEnumerable<ProposedClueResponse>>.Ok(clues.Select(clue =>
            {
                var votes = (clueVotesGrouped.ContainsKey(clue.Id) ? clueVotesGrouped[clue.Id] : (IEnumerable<ClueVote>) new List<ClueVote>())
                .Select(clueVote => {
                    return new ClueVoteResponse
                    {
                        Id = clueVote.Id,
                        ClueId = clueVote.ClueId,
                        PlayerId = clueVote.PlayerId
                    };
                });


                return new ProposedClueResponse 
                { 
                    PlayerId = clue.ClueGiverPlayerId,
                    Id = clue.Id,
                    NumberOfLetters = clue.NumberOfLetters,
                    NumberOfPlayerLetters = clue.NumberOfPlayerLetters,
                    NumberOfNonPlayerLetters = clue.NumberOfNonPlayerLetters,
                    NumberOfBonusLetters = clue.NumberOfBonusLetters,
                    WildcardUsed = clue.WildcardUsed,
                    Votes = votes
                };
            }));
        }
    }
}
