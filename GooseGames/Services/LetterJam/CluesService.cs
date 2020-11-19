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
        private readonly IClueLetterRepository _clueLetterRepository;
        private readonly IClueVoteRepository _clueVoteRepository;
        private readonly ILetterCardRepository _letterCardRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly RequestLogger<CluesService> _logger;

        public CluesService(
            GameService gameService,
            IClueRepository clueRepository,
            IClueLetterRepository clueLetterRepository,
            IClueVoteRepository clueVoteRepository,
            ILetterCardRepository letterCardRepository,
            IGameRepository gameRepository,
            IRoundRepository roundRepository,
            RequestLogger<CluesService> logger)
        {
            _gameService = gameService;
            _clueRepository = clueRepository;
            _clueLetterRepository = clueLetterRepository;
            _clueVoteRepository = clueVoteRepository;
            _letterCardRepository = letterCardRepository;
            _gameRepository = gameRepository;
            _roundRepository = roundRepository;
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

        internal async Task<GenericResponseBase> SubmitClueAsync(SubmitClueRequest request)
        {
            var roundId = request.RoundId;
            if (roundId == null)
            {
                var game = await _gameRepository.GetAsync(request.GameId);
                roundId = game.CurrentRoundId;
            }
            if (roundId == null)
            {
                return GenericResponseBase.Error("Unable to find current round id");
            }

            Round round = await _roundRepository.GetAsync(roundId.Value);
            if (round == null)
            {
                return GenericResponseBase.Error("Unable to find round");
            }

            var requestedClueLetters = request.ClueLetters;
            var clue = new Clue
            {
                Id = Guid.NewGuid(),
                ClueGiverPlayerId = request.PlayerId,
                RoundId = round.Id,
                RoundNumber = round.RoundNumber,
                WildcardUsed = requestedClueLetters.Any(c => c.IsWildCard)
            };
            var clueLetters = new List<ClueLetter>();

            var actualLetterIds = request.ClueLetters.Where(x => x.LetterId.HasValue).Select(x => x.LetterId).ToList();

            var letters = await _letterCardRepository.FilterAsync(c => actualLetterIds.Contains(c.Id));

            var letterIndex = 0;
            foreach (var clueLetter in requestedClueLetters)
            {
                LetterCard letter = null;
                if (!clueLetter.IsWildCard)                
                {
                    letter = letters.FirstOrDefault(l => l.Id == clueLetter.LetterId);
                    if (letter == null)
                    {
                        return GenericResponseBase.Error("Unable to find letter");
                    }
                }

                clueLetters.Add(new ClueLetter
                {
                    Id = Guid.NewGuid(),
                    Clue = clue,
                    Letter = letter?.Letter,
                    PlayerId = letter?.PlayerId,
                    NonPlayerCharacterId = letter?.NonPlayerCharacterId,
                    LetterId = letter?.Id,
                    BonusLetter = letter?.BonusLetter ?? false,
                    LetterIndex = letterIndex,
                    IsWildCard = clueLetter.IsWildCard
                });
                letterIndex++;
            }

            foreach (var letter in letters)
            {
                if (letter.PlayerId.HasValue)
                {
                    clue.NumberOfPlayerLetters += 1;
                }
                else if (letter.NonPlayerCharacterId.HasValue)
                {
                    clue.NumberOfNonPlayerLetters += 1;
                }
                else if (letter.BonusLetter)
                {
                    clue.NumberOfBonusLetters += 1;
                }
            }

            await _clueRepository.InsertAsync(clue);
            await _clueLetterRepository.InsertRangeAsync(clueLetters);

            return GenericResponseBase.Ok();
        }
    }
}
