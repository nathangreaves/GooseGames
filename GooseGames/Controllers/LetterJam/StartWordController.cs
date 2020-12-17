using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using Models.Responses.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.LetterJam
{
    [ApiController]
    [Route("[controller]")]
    public class LetterJamStartWordController : ControllerBase
    {
        private readonly StartWordService _startWordService;
        private readonly RequestLogger<LetterJamStartWordController> _logger;

        public LetterJamStartWordController(
                StartWordService startWordService,
                RequestLogger<LetterJamStartWordController> logger
            )
        {
            _startWordService = startWordService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Configuration")]
        public async Task<GenericResponse<StartWordConfigurationResponse>> GetStartWordConfigurationAsync([FromQuery] PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _startWordService.GetStartWordConfigurationAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<StartWordConfigurationResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [Route("RandomWord")]
        public GenericResponse<RandomWordResponse> GetRandomWord([FromQuery] RandomWordRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = _startWordService.GetRandomWord(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<RandomWordResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        public async Task<GenericResponseBase> PostStartWordAsync(StartWordRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _startWordService.PostStartWordAsync(request);

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
        [Route("PlayerActions")]
        public async Task<GenericResponse<IEnumerable<Models.Responses.LetterJam.PlayerActionResponse>>> GetPlayerActionsAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _startWordService.GetPlayerActionsAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<Models.Responses.LetterJam.PlayerActionResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
