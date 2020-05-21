using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.Werewords;
using Models.Responses;
using Models.Responses.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Werewords
{
    [ApiController]
    [Route("[controller]")]
    public class WerewordsRoundController : ControllerBase
    {
        private readonly RoundService _roundService;
        private readonly RequestLogger<WerewordsRoundController> _logger;

        public WerewordsRoundController(RoundService roundService,
            RequestLogger<WerewordsRoundController> logger)
        {
            _roundService = roundService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Words")]
        public GenericResponse<IEnumerable<string>> GetWords([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = _roundService.GetWordChoiceAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponse<IEnumerable<string>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("Word")]
        public async Task<GenericResponseBase> PostWordAsync(WordChoiceRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.PostWordAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponse<IEnumerable<string>>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
