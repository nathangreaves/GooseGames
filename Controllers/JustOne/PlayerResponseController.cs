using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;

namespace GooseGames.Controllers.JustOne
{
    [Route("[controller]/[action]")]
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

        [ActionName("Info")]
        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetInfoAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                var result = await _playerResponseService.GetPlayerResponseInfoAsync(request);

                _logger.LogTrace("Returned result", result);
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