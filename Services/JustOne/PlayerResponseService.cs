using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PlayerResponseService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRoundRepository _roundRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly RequestLogger<PlayerResponseService> _logger;

        public PlayerResponseService(IPlayerRepository playerRepository, IRoundRepository roundRepository, IResponseRepository responseRepository, RequestLogger<PlayerResponseService> logger)
        {
            _playerRepository = playerRepository;
            _roundRepository = roundRepository;
            _responseRepository = responseRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseInfoAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Getting players responses", request);

            _logger.LogTrace("Getting current round for session");

            var currentRound = await _roundRepository.SingleOrDefaultAsync(p => 
            p.SessionId == request.SessionId 
            && p.ActivePlayerId != null 
            && p.Outcome == RoundOutcomeEnum.Undetermined
            && p.Status == RoundStatusEnum.WaitingForResponses);

            if (currentRound == null)
            {
                _logger.LogError("Unable to find single active round", request);

                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error("Failed to find round");
            }

            _logger.LogTrace("Getting players for session");
            var playersExceptActivePlayer = await _playerRepository.FilterAsync(p => p.SessionId == request.SessionId && p.Id != currentRound.ActivePlayerId);
            var playerIds = playersExceptActivePlayer.Select(player => player.Id).ToList();

            _logger.LogTrace("Getting responses");
            var responses = await _responseRepository.FilterAsync(p => playerIds.Contains(p.PlayerId) && p.RoundId == currentRound.Id);

            _logger.LogTrace("Preparing result");
            var result = playersExceptActivePlayer.Select(p => new PlayerActionResponse
            {
                Id = p.Id,
                PlayerName = p.Name,
                PlayerNumber = p.PlayerNumber,
                HasTakenAction = responses.Any(x => x.PlayerId == p.Id)
            });

            return GenericResponse<IEnumerable<PlayerActionResponse>>.Ok(result);
        }
    }
}
