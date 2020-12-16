using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
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
    public class LetterJamGameEndController : ControllerBase
    {
        private readonly GameEndService _gameEndService;
        private readonly RequestLogger<LetterJamGameEndController> _logger;

        public LetterJamGameEndController(GameEndService gameEndService,
            RequestLogger<LetterJamGameEndController> logger)
        {
            _gameEndService = gameEndService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<GameEndResponse>> GetAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _gameEndService.GetGameEndAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<GameEndResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
