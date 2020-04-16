using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Requests.JustOne;
using Models.Requests.JustOne.Response;
using Models.Responses;
using Models.Responses.JustOne;
using Models.Responses.JustOne.PlayerStatus;
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
        private readonly PlayerHubContext _playerHub;
        private readonly RequestLogger<PlayerResponseService> _logger;

        public PlayerResponseService(RoundService roundService,
            IPlayerRepository playerRepository,
            IPlayerStatusRepository playerStatusRepository,
            IResponseRepository responseRepository,
            IResponseVoteRepository responseVoteRepository,
            PlayerHubContext playerHub, RequestLogger<PlayerResponseService> logger)
        {
            _roundService = roundService;
            _playerRepository = playerRepository;
            _playerStatusRepository = playerStatusRepository;
            _responseRepository = responseRepository;
            _responseVoteRepository = responseVoteRepository;
            _playerHub = playerHub;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerResponses>> GetResponsesAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Getting responses for current round", request);

            _logger.LogTrace("Fetching current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            _logger.LogTrace("Getting responses");
            var responsesDictionary = (await _responseRepository.FilterAsync(r => r.RoundId == round.Id)).ToDictionary(r => r.PlayerId, r => r);

            _logger.LogTrace("Getting players");
            var players = await _playerRepository
                .FilterAsync(player => player.SessionId == request.SessionId);

            _logger.LogTrace("Getting players except active player");
            var playersExceptActivePlayer = players.Where(p => p.Id != round.ActivePlayerId);

            _logger.LogTrace("Getting active player details");
            var activePlayer = players.Single(p => p.Id == round.ActivePlayerId.Value);

            var isActivePlayer = request.PlayerId == round.ActivePlayerId;
            _logger.LogTrace($"Requesting player is active player = {isActivePlayer}");

            var responses = playersExceptActivePlayer.OrderBy(x => x.PlayerNumber).Select(p =>
            {
                var response = responsesDictionary[p.Id];
                var responseInvalid = response.Status == ResponseStatusEnum.AutoInvalid || response.Status == ResponseStatusEnum.ManualInvalid;
                return new PlayerResponse
                {
                    ResponseId = response.Id,
                    PlayerId = p.Id,
                    PlayerName = p.Name,
                    PlayerNumber = p.PlayerNumber,
                    Response = isActivePlayer && responseInvalid && round.Status == RoundStatusEnum.WaitingForLeaderResponse ? null : response.Word,
                    ResponseInvalid = responseInvalid
                };
            }).ToList();

            _logger.LogTrace($"Returning responses", responses);
            return GenericResponse<PlayerResponses>.Ok(new PlayerResponses
            {
                ActivePlayerName = activePlayer.Name,
                ActivePlayerNumber = activePlayer.PlayerNumber,
                WordToGuess = isActivePlayer ? null : round.WordToGuess,
                Responses = responses
            });
        }

        internal async Task<GenericResponse<PlayerResponses>> GetActivePlayerResponseAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Getting active player response for current round", request);

            _logger.LogTrace("Fetching current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            _logger.LogTrace("Getting response");
            var activePlayerResponse = await _responseRepository.SingleOrDefaultAsync(r => r.RoundId == round.Id && r.PlayerId == round.ActivePlayerId.Value);

            _logger.LogTrace("Getting active player details");
            var activePlayer = await _playerRepository
                .GetAsync(round.ActivePlayerId.Value);

            return GenericResponse<PlayerResponses>.Ok(new PlayerResponses
            {
                ActivePlayerName = activePlayer.Name,
                ActivePlayerNumber = activePlayer.PlayerNumber,
                WordToGuess = round.WordToGuess,
                Responses = new List<PlayerResponse> 
                { 
                    new PlayerResponse
                    {
                        ResponseId = activePlayerResponse.Id,
                        PlayerId = activePlayer.Id,
                        PlayerName = activePlayer.Name,
                        PlayerNumber = activePlayer.PlayerNumber,
                        Response = activePlayerResponse.Word
                    }
                }
            });
        }

        internal async Task<GenericResponseBase> SubmitClueAsync(ResponseRequest request)
        {
            _logger.LogTrace($"Submit clue", request);
            _logger.LogTrace($"Getting current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            if (await PlayerHasAlreadySubmittedResponse(request, round))
            {
                _logger.LogWarning($"Active player tried to submit response for round {round.Id}:{round.WordToGuess} more than once", request);
                return GenericResponseBase.Error("Already submitted a guess for current round. Please refresh your page");
            }

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

            return GenericResponseBase.Ok();
        }

        internal async Task SubmitResponseVoteAsync(ResponseVotesRequest request)
        {
            _logger.LogTrace($"Submitting response vote", request);
            _logger.LogTrace($"Getting current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            if (request.ValidResponses.Any())
            {
                var responseVotes = request.ValidResponses.Select(validResponseId =>
                {
                    return new ResponseVote
                    {
                        ResponseId = validResponseId,
                        PlayerId = request.PlayerId
                    };
                });

                _logger.LogTrace($"Inserting response votes");
                await _responseVoteRepository.InsertRangeAsync(responseVotes);
            }
            else
            {
                _logger.LogTrace($"No votes given");
            }

            _logger.LogTrace($"Updating status of player");
            await _playerStatusRepository.UpdateStatusAsync(request.PlayerId, PlayerStatusEnum.PassivePlayerWaitingForClueVotes);

            _logger.LogTrace($"Sending notification of clue submission");
            await _playerHub.SendClueVoteSubmittedAsync(request.SessionId, request.PlayerId);

            _logger.LogTrace($"Progressing round");
            await _roundService.ProgressRoundAsync(round);
        }

        internal async Task<GenericResponseBase> SubmitActivePlayerResponseAsync(ActivePlayerResponseRequest request)
        {
            _logger.LogTrace($"Submit active player response request", request);
            _logger.LogTrace($"Getting current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            if (await PlayerHasAlreadySubmittedResponse(request, round))
            {
                _logger.LogWarning($"User tried to submit response for round {round.Id}:{round.WordToGuess} more than once", request);
                return GenericResponseBase.Error("Already submitted response to current round. Please refresh your page");
            }

            _logger.LogTrace($"Preparing response");
            var response = new Response
            {
                PlayerId = request.PlayerId,
                RoundId = round.Id,
                Status = request.Pass ? ResponseStatusEnum.PassedActivePlayerResponse : ResponseStatusEnum.New,
                Word = request.ResponseWord != null ? request.ResponseWord.ToUpper() : request.ResponseWord
            };

            _logger.LogTrace($"Inserting response");
            await _responseRepository.InsertAsync(response);

            _logger.LogTrace($"Progressing round");
            await _roundService.ProgressRoundAsync(round);

            return GenericResponseBase.Ok();
        }

        internal async Task SubmitActivePlayerResponseVoteAsync(ResponseVotesRequest request)
        {
            _logger.LogTrace($"Submitting active player response vote", request);
            _logger.LogTrace($"Getting current round");
            var round = await _roundService.GetCurrentRoundAsync(request);

            var responseVote = request.ValidResponses.SingleOrDefault();
            if (responseVote != null && responseVote != default(Guid))
            {
                _logger.LogTrace($"Voted: Success");
                _logger.LogTrace($"Inserting response vote");
                await _responseVoteRepository.InsertAsync(new ResponseVote
                {
                    ResponseId = responseVote,
                    PlayerId = request.PlayerId
                });
            }
            else
            {
                _logger.LogTrace($"Voted: Fail");
            }

            _logger.LogTrace($"Updating status of player");
            await _playerStatusRepository.UpdateStatusAsync(request.PlayerId, PlayerStatusEnum.PassivePlayerWaitingForOutcomeVotes);

            _logger.LogTrace($"Sending notification of clue submission");
            await _playerHub.SendResponseVoteSubmittedAsync(request.SessionId, request.PlayerId);

            _logger.LogTrace($"Progressing round");
            await _roundService.ProgressRoundAsync(round);
        }
        private async Task<bool> PlayerHasAlreadySubmittedResponse(PlayerSessionRequest request, Round round)
        {
            var count = await _responseRepository.CountAsync(r => r.PlayerId == request.PlayerId && r.RoundId == round.Id);

            return count > 0;
        }
    }
}
