using GooseGames.Logging;
using GooseGames.Services.Avalon;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Responses.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Avalon
{
    [ApiController]
    [Route("[controller]")]
    public class AvalonPlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly RequestLogger<AvalonPlayerController> _logger;

        public AvalonPlayerController(PlayerService playerService,
            RequestLogger<AvalonPlayerController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        public async Task<GenericResponse<IEnumerable<PlayerResponse>>> GetAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerService.GetPlayerAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<PlayerResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
