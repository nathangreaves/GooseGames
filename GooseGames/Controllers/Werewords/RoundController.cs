using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
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
        public GenericResponse<WordChoiceResponse> GetWords()
        {
            try
            {
                _logger.LogInformation("Received request");

                var result = _roundService.GetWordChoiceAsync();

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponse<WordChoiceResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
