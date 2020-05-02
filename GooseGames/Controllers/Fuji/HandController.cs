using GooseGames.Logging;
using GooseGames.Services.Fuji;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Responses.Fuji.Hands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Fuji
{
    [ApiController]
    [Route("[controller]")]
    public class FujiHandController : ControllerBase
    {
        private readonly HandService _handService;
        private readonly RequestLogger<FujiHandController> _logger;

        public FujiHandController(HandService handService,
            RequestLogger<FujiHandController> logger)
        {
            _handService = handService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<PlayerHand>> GetAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _handService.GetPlayerHandAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<PlayerHand>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
