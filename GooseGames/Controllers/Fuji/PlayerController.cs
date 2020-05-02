using GooseGames.Logging;
using GooseGames.Services.Fuji;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.PlayerDetails;
using Models.Responses;
using Models.Responses.PlayerDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Fuji
{
    [ApiController]
    [Route("[controller]")]
    public class FujiPlayerController : ControllerBase
    {
        private readonly PlayerDetailsService _playerDetailsService;
        private readonly RequestLogger<FujiPlayerController> _logger;

        public FujiPlayerController(PlayerDetailsService playerDetailsService,
            RequestLogger<FujiPlayerController> logger)
        {
            _playerDetailsService = playerDetailsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<GetPlayerDetailsResponse>> GetAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerDetailsService.GetPlayerDetailsAsync(request);

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

        [HttpPatch]
        public async Task<GenericResponseBase> PatchAsync(UpdatePlayerDetailsRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerDetailsService.UpdatePlayerDetailsAsync(request);

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

        [HttpDelete]
        public async Task<GenericResponseBase> DeleteAsync([FromQuery]DeletePlayerRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerDetailsService.DeletePlayerAsync(request);

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
