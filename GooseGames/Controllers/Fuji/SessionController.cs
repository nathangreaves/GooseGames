using GooseGames.Logging;
using GooseGames.Services.Fuji;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.Fuji;
using Models.Responses.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Fuji
{
    [ApiController]
    [Route("[controller]")]
    public class FujiSessionController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly RequestLogger<FujiSessionController> _logger;

        public FujiSessionController(SessionService sessionService,
            RequestLogger<FujiSessionController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<SessionResponse>> GetAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.GetAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<SessionResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("StartSession")]
        public async Task<GenericResponseBase> StartSessionAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.StartSessionAsync(request);

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
    }
}
