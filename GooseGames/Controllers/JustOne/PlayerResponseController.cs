using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.JustOne;
using Models.Requests.JustOne.Response;
using Models.Responses;
using Models.Responses.JustOne;
using Models.Responses.JustOne.Response;

namespace GooseGames.Controllers.JustOne
{
    [Route("[controller]")]
    [ApiController]
    public class JustOnePlayerResponseController : ControllerBase
    {
        private readonly PlayerResponseService _playerResponseService;
        private readonly RequestLogger<JustOnePlayerResponseController> _logger;

        public JustOnePlayerResponseController(PlayerResponseService playerResponseService, RequestLogger<JustOnePlayerResponseController> logger)
        {
            _playerResponseService = playerResponseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<PlayerResponses>> GetAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerResponseService.GetResponsesAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<PlayerResponses>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("SubmitResponse")]
        public async Task<GenericResponseBase> SubmitClueAsync(ResponseRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerResponseService.SubmitClueAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("SubmitResponseVote")]
        public async Task<GenericResponseBase> SubmitClueVoteAsync(ResponseVotesRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerResponseService.SubmitResponseVoteAsync(request);

                var result = GenericResponseBase.Ok();

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("SubmitActivePlayerResponse")]
        public async Task<GenericResponseBase> SubmitActivePlayerResponseAsync(ActivePlayerResponseRequest request) 
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerResponseService.SubmitActivePlayerResponseAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [Route("ActivePlayerResponse")]
        public async Task<GenericResponse<PlayerResponse>> GetActivePlayerResponseAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerResponseService.GetActivePlayerResponseAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<PlayerResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("SubmitActivePlayerResponseVote")]
        public async Task<GenericResponseBase> SubmitActivePlayerResponseVoteAsync(ResponseVotesRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerResponseService.SubmitActivePlayerResponseVoteAsync(request);

                var result = GenericResponseBase.Ok();

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}