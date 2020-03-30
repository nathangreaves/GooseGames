using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Requests.JustOne;
using Models.Requests.JustOne.Response;
using Models.Responses;
using Models.Responses.JustOne;
using Models.Responses.JustOne.Response;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PlayerResponseService
    {
        private readonly RoundService _roundService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerStatusRepository _playerStatusRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly IResponseVoteRepository _responseVoteRepository;
        private readonly IHubContext<PlayerHub> _playerHub;
        private readonly RequestLogger<PlayerResponseService> _logger;

        public PlayerResponseService(RoundService roundService, 
            IPlayerRepository playerRepository,
            IPlayerStatusRepository playerStatusRepository, 
            IResponseRepository responseRepository, 
            IResponseVoteRepository responseVoteRepository,
            IHubContext<PlayerHub> playerHub, RequestLogger<PlayerResponseService> logger)
        {
            _roundService = roundService;
            _playerRepository = playerRepository;
            _playerStatusRepository = playerStatusRepository;
            _responseRepository = responseRepository;
            _responseVoteRepository = responseVoteRepository;
            _playerHub = playerHub;
            _logger = logger;
        }

        internal async Task<GenericResponse<IEnumerable<PlayerResponse>>> GetResponsesAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Getting responses for current round", request);

            _logger.LogTrace("Fetching current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            _logger.LogTrace("Getting responses");
            var responsesDictionary = (await _responseRepository.FilterAsync(r => r.RoundId == round.Id)).ToDictionary(r => r.PlayerId, r => r);

            _logger.LogTrace("Getting players");
            var playersExceptActivePlayer = await _playerRepository
                .FilterAsync(player => player.SessionId == request.SessionId && player.Id != round.ActivePlayerId);

            var isActivePlayer = request.PlayerId == round.ActivePlayerId;
            _logger.LogTrace($"Requesting player is active player = {isActivePlayer}");

            var result = playersExceptActivePlayer.Select(p =>
            {
                var response = responsesDictionary[p.Id];
                return new PlayerResponse
                {
                    ResponseId = response.Id,
                    PlayerId = p.Id,
                    PlayerName = p.Name,
                    PlayerNumber = p.PlayerNumber,
                    Response = isActivePlayer ? null : response.Word,
                    ResponseInvalid = response.Status == ResponseStatusEnum.AutoInvalid || response.Status == ResponseStatusEnum.ManualInvalid
                };
            }).ToList();

            _logger.LogTrace($"Returning response", result);
            return GenericResponse<IEnumerable<PlayerResponse>>.Ok(result);
        }

        internal async Task SubmitClueAsync(ResponseRequest request)
        {
            _logger.LogTrace($"Submit clue", request);
            _logger.LogTrace($"Getting current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            _logger.LogTrace($"Preparing response");
            var response = new Response
            {
                PlayerId = request.PlayerId,
                RoundId = round.Id,
                Status = ResponseStatusEnum.New,
                Word = request.ResponseWord
            };

            _logger.LogTrace($"Inserting response");
            await _responseRepository.InsertAsync(response);

            _logger.LogTrace($"Updating status of player");
            await _playerStatusRepository.UpdateStatusAsync(request.PlayerId, PlayerStatusEnum.PassivePlayerWaitingForClues);

            _logger.LogTrace($"Sending notification of clue submission");
            await _playerHub.SendClueSubmittedAsync(request.SessionId, request.PlayerId);

            _logger.LogTrace($"Progressing round");
            await _roundService.ProgressRoundAsync(round);
        }


        internal async Task SubmitResponseVoteAsync(ResponseVotesRequest request)
        {
            _logger.LogTrace($"Submitting response vote", request);
            _logger.LogTrace($"Getting current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            var responseVotes = request.ValidResponses.Select(validResponseId => {
                return new ResponseVote { 
                    ResponseId = validResponseId,
                    PlayerId = request.PlayerId
                };
            });

            _logger.LogTrace($"Inserting response votes");
            await _responseVoteRepository.InsertRangeAsync(responseVotes);            

            _logger.LogTrace($"Updating status of player");
            await _playerStatusRepository.UpdateStatusAsync(request.PlayerId, PlayerStatusEnum.PassivePlayerWaitingForClueVotes);

            _logger.LogTrace($"Sending notification of clue submission");
            await _playerHub.SendClueVoteSubmittedAsync(request.SessionId, request.PlayerId);

            _logger.LogTrace($"Progressing round");
            await _roundService.ProgressRoundAsync(round);
        }
    }
}
