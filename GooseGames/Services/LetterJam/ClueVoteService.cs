using GooseGames.Hubs;
using GooseGames.Logging;
using Models.Requests.LetterJam;
using Models.Responses;
using RepositoryInterface.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.LetterJam
{
    public class ClueVoteService
    {
        private readonly IClueVoteRepository _clueVoteRepository;
        private readonly IGameRepository _gameRepository;
        private readonly LetterJamHubContext _letterJamHubContext;
        private readonly RequestLogger<ClueVoteService> _logger;

        public ClueVoteService(IClueVoteRepository clueVoteRepository,
            IGameRepository gameRepository,
            LetterJamHubContext letterJamHubContext,
            RequestLogger<ClueVoteService> logger)
        {
            _clueVoteRepository = clueVoteRepository;
            _gameRepository = gameRepository;
            _letterJamHubContext = letterJamHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponseBase> PostAsync(ClueRequest request)
        {
            var roundId = request.RoundId;
            if (!roundId.HasValue)
            {
                roundId = await _gameRepository.GetPropertyAsync(request.GameId, g => g.CurrentRoundId);
            }
            if (!roundId.HasValue)
            {
                return GenericResponseBase.Error("Unable to find round");
            }

            var existingClueVotes = await _clueVoteRepository.FilterAsync(cV => cV.RoundId == roundId.Value && cV.PlayerId == request.PlayerId);
            foreach (var existingClueVote in existingClueVotes)
            {
                await _clueVoteRepository.DeleteAsync(existingClueVote);
                await _letterJamHubContext.SendRemoveVoteAsync(request, existingClueVote.ClueId);
            }

            if (request.ClueId.HasValue)
            {
                await _clueVoteRepository.InsertAsync(new Entities.LetterJam.ClueVote
                {
                    Id = Guid.NewGuid(),
                    ClueId = request.ClueId.Value,
                    PlayerId = request.PlayerId,
                    RoundId = roundId.Value
                });
                await _letterJamHubContext.SendAddVoteAsync(request, request.ClueId.Value);
            }
            return GenericResponseBase.Ok();
        }
    }
}
