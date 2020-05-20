using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Werewords
{
    [ApiController]
    [Route("[controller]")]
    public class WerewordsSessionController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly RequestLogger<WerewordsSessionController> _logger;

        public WerewordsSessionController(SessionService sessionService,
            RequestLogger<WerewordsSessionController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        //[HttpGet]
        //public async Task<GenericResponse<SessionResponse>> GetAsync([FromQuery]PlayerSessionRequest request)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Received request", request);

        //        var result = await _sessionService.GetAsync(request);

        //        _logger.LogInformation("Returned result", result);

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        var errorGuid = Guid.NewGuid();
        //        _logger.LogError($"Unknown Error {errorGuid}", e, request);
        //        return GenericResponse<SessionResponse>.Error($"Unknown Error {errorGuid}");
        //    }
        //}

        [HttpPost]
        public async Task<GenericResponse<NewSessionResponse>> PostAsync(NewSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.CreateSessionAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<NewSessionResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPatch]
        public async Task<GenericResponse<JoinSessionResponse>> PatchAsync(JoinSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.JoinSessionAsync(request);

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

        //[HttpPost]
        //[Route("CreateTestSession")]
        //public async Task<GenericResponse<IEnumerable<JoinSessionResponse>>> CreateTestSessionAsync()
        //{
        //    try
        //    {
        //        _logger.LogInformation("Received request");

        //        string sessionPassword = Guid.NewGuid().ToString();
        //        var result = await _sessionService.CreateTestSessionAsync();

        //        _logger.LogInformation("Returned result");

        //        return GenericResponse<IEnumerable<JoinSessionResponse>>.Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        var errorGuid = Guid.NewGuid();
        //        _logger.LogError($"Unknown Error {errorGuid}", e);
        //        return GenericResponse<IEnumerable<JoinSessionResponse>>.Error($"Unknown Error {errorGuid}");
        //    }
        //}
    }    
}
