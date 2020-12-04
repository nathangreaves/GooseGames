using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.LetterJam.Enums;
using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Responses.LetterJam;

namespace GooseGames.Controllers.LetterJam
{
    [Route("[controller]")]
    [ApiController]
    public class LetterJamPlayerStatusController : ControllerBase
    {
        private readonly PlayerStatusService _playerStatusService;
        private readonly RequestLogger<LetterJamPlayerStatusController> _logger;

        public LetterJamPlayerStatusController(
            PlayerStatusService playerStatusService,
            RequestLogger<LetterJamPlayerStatusController> logger
            )
        {
            _playerStatusService = playerStatusService;
            _logger = logger;
        }

        [HttpGet]
        [Route(nameof(PlayerStatus.InLobby))]
        public async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidateLobbyAsync([FromQuery]PlayerSessionPossibleGameRequest request)
        {
            return await ValidateStatus(request, PlayerStatus.InLobby);
        }
        [HttpGet]
        [Route(nameof(PlayerStatus.ConstructingWord))]
        public async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidateConstructingWordAsync([FromQuery]PlayerSessionPossibleGameRequest request)
        {
            return await ValidateStatus(request, PlayerStatus.ConstructingWord);
        }
        [HttpGet]
        [Route(nameof(PlayerStatus.WaitingForFirstRound))]
        public async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidateWaitingForFirstRoundAsync([FromQuery]PlayerSessionPossibleGameRequest request)
        {
            return await ValidateStatus(request, PlayerStatus.WaitingForFirstRound);
        }
        [HttpGet]
        [Route(nameof(PlayerStatus.ProposingClues))]
        public async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidateProposingCluesAsync([FromQuery]PlayerSessionPossibleGameRequest request)
        {
            return await ValidateStatus(request, PlayerStatus.ProposingClues);
        }
        [HttpGet]
        [Route(nameof(PlayerStatus.ReceivedClue))]
        public async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidateReceivedClueAsync([FromQuery]PlayerSessionPossibleGameRequest request)
        {
            return await ValidateStatus(request, PlayerStatus.ReceivedClue);
        }
        [HttpGet]
        [Route(nameof(PlayerStatus.ReadyForNextRound))]
        public async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidateReadyForNextRoundAsync([FromQuery]PlayerSessionPossibleGameRequest request)
        {
            return await ValidateStatus(request, PlayerStatus.ReadyForNextRound);
        }


        [HttpPost]
        [Route("WaitingForNextRound")]
        public async Task<GenericResponseBase> SetWaitingForNextRoundAsync(PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation($"Received request", request);

                var result = await _playerStatusService.SetWaitingForNextRoundAsync(request);

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
        [Route("UndoWaitingForNextRound")]
        public async Task<GenericResponseBase> UndoWaitingForNextRoundAsync(PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation($"Received request", request);

                var result = await _playerStatusService.SetUndoWaitingForNextRoundAsync(request);

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

        private async Task<GenericResponse<LetterJamPlayerStatusValidationResponse>> ValidateStatus(PlayerSessionPossibleGameRequest request, PlayerStatusId lobbyStatus)
        {
            try
            {
                _logger.LogInformation($"Received request status: {lobbyStatus} = {PlayerStatus.TryGetDescription(lobbyStatus)}", request);

                var result = await _playerStatusService.ValidatePlayerStatusAsync(request, lobbyStatus);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<LetterJamPlayerStatusValidationResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}