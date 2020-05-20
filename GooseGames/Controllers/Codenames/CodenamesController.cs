using GooseGames.Logging;
using GooseGames.Services.Codenames;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.Codenames;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Responses.Codenames;

namespace GooseGames.Controllers.Codenames
{
    [ApiController]
    [Route("[controller]")]
    public class CodenamesController : ControllerBase
    {
        private readonly CodenamesService _codenamesService;
        private readonly RequestLogger<CodenamesController> _logger;

        public CodenamesController(CodenamesService codenamesService,
            RequestLogger<CodenamesController> logger)
        {
            _codenamesService = codenamesService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<Session>> GetAsync([FromQuery] SessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _codenamesService.GetAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<Session>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<GenericResponseBase> PostAsync(RefreshSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _codenamesService.RefreshWordsAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<Session>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("Reveal")]
        public async Task<GenericResponseBase> RevealAsync(RevealWordRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _codenamesService.RevealWordAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<Session>.Error($"Unknown Error {errorGuid}");
            }
        }


        [HttpPost]
        [Route("Pass")]
        public async Task<GenericResponseBase> PassAsync(PassRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _codenamesService.PassTurnAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<Session>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
