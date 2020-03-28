using Entities.JustOne.Enums;
using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne.PlayerStatus;
using System;
using System.Threading.Tasks;

namespace GooseGames.Controllers.JustOne
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class JustOnePlayerStatusController : ControllerBase
    {
        private readonly PlayerStatusService _playerStatusService;
        private readonly RequestLogger<JustOnePlayerStatusController> _logger;

        public JustOnePlayerStatusController(PlayerStatusService playerStatusService, RequestLogger<JustOnePlayerStatusController> logger)
        {
            _playerStatusService = playerStatusService;
            _logger = logger;
        }

        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.New))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateNewAsync([FromQuery] PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.New);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.InLobby))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateLobbyAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.InLobby);
        }
        [HttpPost]
        [ActionName(nameof(PlayerStatusEnum.InLobby))]
        public async Task<GenericResponse<bool>> SetLobbyAsync(PlayerIdRequest request)
        {
            return await SetStatusAsync(request, PlayerStatusEnum.InLobby);
        }
        [HttpGet]
        [ActionName("Waiting")]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateWaitingAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.Waiting);
        }
        [HttpPost]
        [ActionName("Waiting")]
        public async Task<GenericResponse<bool>> SetWaitingAsync(PlayerIdRequest request)
        {
            return await SetStatusAsync(request, PlayerStatusEnum.Waiting);
        }

        private async Task<GenericResponse<bool>> SetStatusAsync(PlayerIdRequest request, Guid status)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                await _playerStatusService.UpdatePlayerStatusAsync(request.PlayerId, status);

                var result = NewResponse.Ok(true);

                _logger.LogTrace("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return NewResponse.Error<bool>($"Unknown Error {errorGuid}");
            }
        }


        private async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateStatus(PlayerSessionRequest request, Guid lobbyStatus)
        {
            try
            {
                _logger.LogTrace("Received request", request);

                var result = await _playerStatusService.ValidatePlayerStatusAsync(request, lobbyStatus);

                _logger.LogTrace("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return NewResponse.Error<PlayerStatusValidationResponse>($"Unknown Error {errorGuid}");
            }
        }
    }
}