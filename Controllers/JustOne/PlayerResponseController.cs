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
        public async Task<GenericResponse<IEnumerable<PlayerResponse>>> GetAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                var result = await _playerResponseService.GetResponsesAsync(request);

                _logger.LogTrace("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<PlayerResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("SubmitClue")]
        public async Task<GenericResponseBase> SubmitClueAsync(ResponseRequest request)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                await _playerResponseService.SubmitClueAsync(request);

                var result = GenericResponseBase.Ok();

                _logger.LogTrace("Returned result", result);
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
        [Route("SubmitClueVote")]
        public async Task<GenericResponseBase> SubmitClueVoteAsync(ResponseVotesRequest request)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                await _playerResponseService.SubmitResponseVoteAsync(request);

                var result = GenericResponseBase.Ok();

                _logger.LogTrace("Returned result", result);
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