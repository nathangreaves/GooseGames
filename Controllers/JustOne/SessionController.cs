using Microsoft.AspNetCore.Mvc;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Services.JustOne;
using Microsoft.Extensions.Logging;
using NLog;
using GooseGames.Extensions;
using GooseGames.Logging;

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
        public async Task<GenericResponse<NewSessionResponse>> PostAsync(NewSessionRequest request)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                var result = await _sessionService.CreateSessionAsync(request);

                _logger.LogTrace("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return NewResponse.Error<NewSessionResponse>($"Unknown Error {errorGuid}");
            }
        }

        [HttpPatch]
        public async Task<GenericResponse<JoinSessionResponse>> PatchAsync(JoinSessionRequest request)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                var result = await _sessionService.JoinSessionAsync(request);

                _logger.LogTrace("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return NewResponse.Error<JoinSessionResponse>($"Unknown Error {errorGuid}");
            }
        }
    }
}
