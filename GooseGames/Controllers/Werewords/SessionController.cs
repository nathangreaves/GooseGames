using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.PlayerDetails;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.PlayerDetails;
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

        [HttpPost]
        [Route("CreateTestSession")]
        public async Task<GenericResponse<IEnumerable<JoinSessionResponse>>> CreateTestSessionAsync(JoinSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request");

                var result = await _sessionService.CreateTestSessionAsync(request);

                _logger.LogInformation("Returned result");

                return GenericResponse<IEnumerable<JoinSessionResponse>>.Ok(result);
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponse<IEnumerable<JoinSessionResponse>>.Error($"Unknown Error {errorGuid} : {e.Message}");
            }
        }


        [HttpPost]
        [Route("UpdatePlayerDetails")]
        public async Task<GenericResponseBase> UpdatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.UpdatePlayerDetailsAsync(request);

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

        [HttpGet]
        [Route("Lobby")]
        public async Task<GenericResponse<GetPlayerDetailsResponse>> GetLobbyAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.GetPlayerDetailsAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<GetPlayerDetailsResponse>.Error($"Unknown Error {errorGuid}");
            }
        }


        [HttpDelete]
        [Route("KickPlayer")]
        public async Task<GenericResponseBase> KickPlayerAsync([FromQuery]DeletePlayerRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.DeletePlayerAsync(request);

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

        [HttpPost]
        [Route("Again")]
        public async Task<GenericResponseBase> AgainAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _sessionService.AgainAsync(request);

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
