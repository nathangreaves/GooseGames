using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.JustOne;
using Models.Requests.JustOne.Round;
using Models.Responses;
using Models.Responses.JustOne.Round;

namespace GooseGames.Controllers.JustOne
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class JustOneRoundController : ControllerBase
    {
        private readonly RoundService _roundService;
        private readonly RequestLogger<JustOneRoundController> _logger;

        public JustOneRoundController(
            RoundService roundService,
            RequestLogger<JustOneRoundController> logger)
        {
            _roundService = roundService;
            _logger = logger;
        }

        [ActionName("PassivePlayerInfo")]
        [HttpGet]
        public async Task<GenericResponse<PassivePlayerRoundInformationResponse>> PassivePlayerInfoAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.GetPassivePlayerRoundInfoAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<PassivePlayerRoundInformationResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [ActionName("Outcome")]
        public async Task<GenericResponse<RoundOutcomeResponse>> GetRoundOutcome([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.GetRoundOutcomeAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<RoundOutcomeResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}