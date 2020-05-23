using Entities.Werewords.Enums;
using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.Werewords;
using Models.Responses;
using Models.Responses.Werewords;
using Models.Responses.Werewords.Round;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Werewords
{
    [ApiController]
    [Route("[controller]")]
    public class WerewordsRoundController : ControllerBase
    {
        private readonly RoundService _roundService;
        private readonly RequestLogger<WerewordsRoundController> _logger;

        public WerewordsRoundController(RoundService roundService,
            RequestLogger<WerewordsRoundController> logger)
        {
            _roundService = roundService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Words")]
        public GenericResponse<IEnumerable<string>> GetWords([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = _roundService.GetWordChoiceAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponse<IEnumerable<string>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [Route("SecretWord")]
        public async Task<GenericResponse<SecretWordResponse>> GetSecretWordAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.GetSecretWordAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponse<SecretWordResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("SecretWord")]
        public async Task<GenericResponseBase> PostWordAsync(WordChoiceRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.PostWordAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [Route("Day")]
        public async Task<GenericResponse<DayResponse>> GetDayAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.GetDayAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<DayResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("PlayerResponse")]
        public async Task<GenericResponseBase> PostWordAsync(SubmitPlayerResponseRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.SubmitPlayerResponseAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("Start")]
        public async Task<GenericResponseBase> StartAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.StartAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("VoteAsSeer")]
        public async Task<GenericResponseBase> VoteAsSeerAsync(SubmitVoteRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.SubmitVoteAsync(request, PlayerVoteTypeEnum.Seer);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("VoteAsWerewolf")]
        public async Task<GenericResponseBase> VoteAsWerewolfAsync(SubmitVoteRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.SubmitVoteAsync(request, PlayerVoteTypeEnum.Werewolf);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponseBase.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [Route("Outcome")]
        public async Task<GenericResponse<RoundOutcomeResponse>> GetOutcomeAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _roundService.GetOutcomeAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<RoundOutcomeResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
