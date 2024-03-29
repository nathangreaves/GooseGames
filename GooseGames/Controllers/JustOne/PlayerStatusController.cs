﻿using Entities.JustOne.Enums;
using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.JustOne.Round;
using Models.Responses;
using Models.Responses.PlayerStatus;
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

        public JustOnePlayerStatusController(PlayerStatusService playerStatusService, 
            RequestLogger<JustOnePlayerStatusController> logger)
        {
            _playerStatusService = playerStatusService;
            _logger = logger;
        }

        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.InLobby))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateLobbyAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation($"Received lobby status", request);

                var result = await _playerStatusService.ValidateGlobalPlayerStatusLobbyAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);

                return GenericResponse<PlayerStatusValidationResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.RoundWaiting))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateWaitingAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.RoundWaiting);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerClue))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerClueAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerClue);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerWaitingForClues))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerWaitingForCluesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerWaitingForClues);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerClueVote))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerClueVoteAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerClueVote);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerWaitingForClueVotes))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerWaitingForClueVotesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerWaitingForClueVotes);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerWaitingForActivePlayer))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerWaitingForActivePlayerAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerWaitingForActivePlayer);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerOutcome))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerOutcomeAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerOutcome);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerOutcomeVote))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerOutcomeVoteAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerOutcomeVote);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerWaitingForOutcomeVotes))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidatePassivePlayerWaitingForOutcomeVotesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.PassivePlayerWaitingForOutcomeVotes);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.ActivePlayerWaitingForClues))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateActivePlayerWaitingForCluesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.ActivePlayerWaitingForClues);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.ActivePlayerWaitingForVotes))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateActivePlayerWaitingForVotesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.ActivePlayerWaitingForVotes);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.ActivePlayerGuess))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateActivePlayerGuessAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.ActivePlayerGuess);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.ActivePlayerWaitingForOutcomeVotes))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateActivePlayerWaitingForOutcomeVotesAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.ActivePlayerWaitingForOutcomeVotes);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.ActivePlayerOutcome))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateActivePlayerOutcomeAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.ActivePlayerOutcome);
        }
        [HttpGet]
        [ActionName(nameof(PlayerStatusEnum.Rejoining))]
        public async Task<GenericResponse<PlayerStatusValidationResponse>> ValidateRejoiningAsync([FromQuery]PlayerSessionRequest request)
        {
            return await ValidateStatus(request, PlayerStatusEnum.Rejoining);
        }


        [HttpPost]
        [ActionName(nameof(PlayerStatusEnum.InLobby))]
        public async Task<GenericResponseBase> SetLobbyAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.SetLobbyAsync(request);

                var result = GenericResponseBase.Ok();

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
        [ActionName(nameof(PlayerStatusEnum.RoundWaiting))]
        public async Task<GenericResponseBase> SetWaitingAsync(PlayerSessionRoundRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerStatusToRoundWaitingAsync(request);

                var result = GenericResponseBase.Ok();

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
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerClue))]
        public async Task<GenericResponseBase> SetPassivePlayerClueAsync(PlayerSessionRoundRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerStatusToPassivePlayerClueAsync(request);

                var result = GenericResponseBase.Ok();

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
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerClueVote))]
        public async Task<GenericResponseBase> SetPassivePlayerClueVoteAsync(PlayerSessionRoundRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerStatusToPassivePlayerClueVoteAsync(request);

                var result = GenericResponseBase.Ok();

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
        [ActionName(nameof(PlayerStatusEnum.PassivePlayerOutcomeVote))]
        public async Task<GenericResponseBase> SetPassivePlayerOutcomeVoteAsync(PlayerSessionRoundRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                await _playerStatusService.UpdatePlayerStatusToPassivePlayerOutcomeVoteAsync(request);

                var result = GenericResponseBase.Ok();

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
                return GenericResponse<PlayerStatusValidationResponse>.Error($"Unknown Error {errorGuid}");
            }
        }


    }
}