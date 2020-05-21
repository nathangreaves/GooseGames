using Entities.Werewords.Enums;
using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Responses.PlayerStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Werewords
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class WerewordsPlayerStatusController : ControllerBase
    {
        private readonly PlayerStatusService _playerStatusService;
        private readonly RequestLogger<WerewordsPlayerStatusController> _logger;

        public WerewordsPlayerStatusController(PlayerStatusService playerStatusService, RequestLogger<WerewordsPlayerStatusController> logger)
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
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.RoundWaiting))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateWaitingAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.RoundWaiting);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.NightRevealSecretRole))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatNightRevealSecretRoleAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.NightRevealSecretRole);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.NightWaitingForPlayersToCheckRole))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateNightWaitingForPlayersToCheckRoleAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.NightWaitingForPlayersToCheckRole);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.NightMayorPickSecretWord))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateNightMayorPickSecretWordAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.NightMayorPickSecretWord);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.NightWaitingForMayor))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateNightWaitingForMayorAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.NightWaitingForMayor);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.NightSecretWord))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateNightSecretWordAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.NightSecretWord);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.NightWaitingToWake))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateNightWaitingToWakeAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.NightWaitingToWake);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.DayMayor))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateDayMayorAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.DayMayor);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.DayPassive))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateDayPassiveAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.DayPassive);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.DayActive))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateDayActiveAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.DayActive);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.DayWaitingForVotes))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateDayWaitingForVotesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.DayWaitingForVotes);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.DayVotingOnWerewolves))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateDayVotingOnWerewolvesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.DayVotingOnWerewolves);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.DayVotingOnSeer))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateDayVotingOnSeerAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.DayVotingOnSeer);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.DayOutcome))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateDayOutcomeAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.DayOutcome);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.WaitingForNextRound))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateWaitingForNextRoundAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.WaitingForNextRound);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.Rejoining))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateRejoiningAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.Rejoining);
        }


        [HttpPost]
        [ActionName(nameof(PlayerStatusEnum.InLobby))]
        public async Task<GenericResponseBase> SetLobbyAsync(PlayerIdRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerToStatusAsync(request.PlayerId, PlayerStatusEnum.InLobby);

                _logger.LogInformation("Returned result");
                return GenericResponseBase.Ok();
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [ActionName("TransitionNight")]
        public async Task<GenericResponse<string>> TransitionNightAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerStatusService.TransitionNightAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<string>.Error($"Unknown Error {errorGuid}");
            }
        }

        //[HttpPost]
        //[ActionName(nameof(PlayerStatusEnum.RoundWaiting))]
        //public async Task<GenericResponse<bool>> SetWaitingAsync(PlayerSessionRoundRequest request)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Received request", request);

        //        await _playerStatusService.UpdatePlayerStatusToRoundWaitingAsync(request);

        //        var result = NewResponse.Ok(true);

        //        _logger.LogInformation("Returned result", result);
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        var errorGuid = Guid.NewGuid();
        //        _logger.LogError($"Unknown Error {errorGuid}", e, request);
        //        return NewResponse.Error<bool>($"Unknown Error {errorGuid}");
        //    }
        //}

        [HttpPost]
        [ActionName(nameof(PlayerStatusEnum.NightWaitingForPlayersToCheckRole))]
        public async Task<GenericResponse<bool>> SetNightWaitingForPlayersToCheckRoleAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerToStatusAndProgressRoundAsync(request, PlayerStatusEnum.NightWaitingForPlayersToCheckRole);

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

        [HttpPost]
        [ActionName(nameof(PlayerStatusEnum.NightWaitingToWake))]
        public async Task<GenericResponse<bool>> SetNightWaitingToWakeAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerToStatusAndProgressRoundAsync(request, PlayerStatusEnum.NightWaitingToWake);

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

        [HttpPost]
        [ActionName(nameof(PlayerStatusEnum.DayWaitingForVotes))]
        public async Task<GenericResponse<bool>> SetPassivePlayerOutcomeVoteAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerToStatusAndProgressRoundAsync(request, PlayerStatusEnum.DayWaitingForVotes);

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


        private async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateStatus(PlayerSessionRequest request, Guid lobbyStatus)
        {
            try
            {
                _logger.LogInformation($"Received request status: {lobbyStatus} = {PlayerStatusEnum.TryGetDescription(lobbyStatus)}", request);

                var result = await _playerStatusService.ValidatePlayerStatusAsync(request, lobbyStatus);

                _logger.LogInformation("Returned result", result);
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
