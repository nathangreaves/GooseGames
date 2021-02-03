using GooseGames.Logging;
using GooseGames.Services.Avalon;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.Avalon;
using Models.Responses;
using Models.Responses.PlayerStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Avalon
{
    [ApiController]
    [Route("[controller]")]
    public class AvalonLobbyController : ControllerBase
    {

        private readonly LobbyService _lobbyService;
        private readonly RequestLogger<AvalonLobbyController> _logger;

        public AvalonLobbyController(
                LobbyService lobbyService,
                RequestLogger<AvalonLobbyController> logger
            )
        {
            _lobbyService = lobbyService;
            _logger = logger;
        }

        [HttpPost]
        [Route("StartSession")]
        public async Task<GenericResponseBase> StartSessionAsync(StartSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _lobbyService.StartSessionAsync(request);

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
        [Route("ValidateStatus")]
        public async Task<GenericResponse<PlayerStatusGameValidationResponse>> ValidatePlayerStatusAsync([FromQuery]PlayerSessionPossibleGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _lobbyService.ValidatePlayerStatusAsync(request, Enums.Avalon.PlayerStatusEnum.InLobby);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<PlayerStatusGameValidationResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
