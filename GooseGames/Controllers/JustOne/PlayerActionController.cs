using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Responses.JustOne;

namespace GooseGames.Controllers.JustOne
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class JustOnePlayerActionController : ControllerBase
    {
        private readonly PlayerActionInformationService _playerActionInfoService;
        private readonly RequestLogger<JustOnePlayerActionController> _logger;

        public JustOnePlayerActionController(PlayerActionInformationService playerActionInfoService, RequestLogger<JustOnePlayerActionController> logger)
        {
            _playerActionInfoService = playerActionInfoService;
            _logger = logger;
        }

        [HttpGet]
        [ActionName("ResponseInfo")]
        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseInfoAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerActionInfoService.GetPlayerResponseInfoAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [ActionName("ResponseVoteInfo")]
        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseVoteInfoAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerActionInfoService.GetPlayerResponseVoteInfoAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }


        [HttpGet]
        [ActionName("ResponseOutcomeVoteInfo")]
        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayerResponseOutcomeVoteInfoAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerActionInfoService.GetPlayerResponseOutcomeVoteInfoAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<PlayerActionResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [ActionName("WaitingForRound")]
        public async Task<GenericResponse<IEnumerable<PlayerActionResponse>>> GetPlayersWaitingForRound([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerActionInfoService.GetPlayersWaitingForRoundAsync(request);

                _logger.LogInformation("Returned result", result);
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