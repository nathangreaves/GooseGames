using Microsoft.AspNetCore.Mvc;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Services.JustOne;
using Microsoft.Extensions.Logging;
using NLog;
using GooseGames.Extensions;
using GooseGames.Logging;
using Models.Requests;
using Models.Requests.Sessions;
using Models.Responses.Sessions;
using Models.Requests.JustOne;

namespace GooseGames.Controllers.JustOne
{
    [ApiController]
    [Route("[controller]")]
    public class JustOneSessionController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly RequestLogger<JustOneSessionController> _logger;

        public JustOneSessionController(SessionService sessionService, RequestLogger<JustOneSessionController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpPost]
        [Route("StartSession")]
        public async Task<GenericResponseBase> StartSessionAsync(StartSessionRequest request)
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
