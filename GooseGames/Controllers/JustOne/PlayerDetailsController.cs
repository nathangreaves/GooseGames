using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.JustOne;
using Models.Requests.JustOne.PlayerDetails;
using Models.Requests.PlayerDetails;
using Models.Responses;
using Models.Responses.JustOne.PlayerDetails;
using Models.Responses.PlayerDetails;

namespace GooseGames.Controllers.JustOne
{
    [Route("[controller]")]
    [ApiController]
    public class JustOnePlayerDetailsController : ControllerBase
    {
        private readonly PlayerDetailsService _playerDetailsService;
        private readonly PlayerStatusService _playerStatusService;
        private readonly RequestLogger<JustOnePlayerDetailsController> _logger;

        public JustOnePlayerDetailsController(PlayerDetailsService playerDetailsService, RequestLogger<JustOnePlayerDetailsController> logger)
        {
            _playerDetailsService = playerDetailsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<GetPlayerDetailsResponse>> GetAsync([FromQuery]GetPlayerDetailsRequest request)
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
                return NewResponse.Error<GetPlayerDetailsResponse>($"Unknown Error {errorGuid}");
            }
        }

        [HttpPatch]
        public async Task<GenericResponse<UpdatePlayerDetailsResponse>> PatchAsync(UpdatePlayerDetailsRequest request)
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
                return NewResponse.Error<UpdatePlayerDetailsResponse>($"Unknown Error {errorGuid}");
            }
        }

        [HttpDelete]
        public async Task<GenericResponse<bool>> DeleteAsync([FromQuery]DeletePlayerRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerDetailsService.DeletePlayerAsync(request);

                var result = NewResponse.Ok(true);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return NewResponse.Error<bool>($"Unknown Error {errorGuid}");
            }
        }
    }
}