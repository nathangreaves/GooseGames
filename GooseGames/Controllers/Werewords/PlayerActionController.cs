using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Werewords
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class WerewordsPlayerActionController : ControllerBase
    {
        private readonly PlayerActionInformationService _playerActionInfoService;
        private readonly RequestLogger<WerewordsPlayerActionController> _logger;

        public WerewordsPlayerActionController(PlayerActionInformationService playerActionInfoService, RequestLogger<WerewordsPlayerActionController> logger)
        {
            _playerActionInfoService = playerActionInfoService;
            _logger = logger;
        }

        [HttpGet]
        [ActionName("PlayersAwake")]
        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseInfoAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerActionInfoService.GetAwakePlayersAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
