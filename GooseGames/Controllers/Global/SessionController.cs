using GooseGames.Logging;
using GooseGames.Services.Global;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Global
{
    [ApiController]
    [Route("[controller]")]
    public class GlobalSessionController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly RequestLogger<GlobalSessionController> _logger;
        public GlobalSessionController(SessionService sessionService,
            RequestLogger<GlobalSessionController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<GenericResponse<JoinSessionResponse>> PostAsync(JoinSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.CreateOrJoinSessionAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<JoinSessionResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
