using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using Models.Responses.LetterJam;

namespace GooseGames.Controllers.LetterJam
{
    [Route("[controller]")]
    [ApiController]
    public class LetterJamCluesController : ControllerBase
    {
        private readonly CluesService _cluesService;
        private readonly RequestLogger<LetterJamCluesController> _logger;

        public LetterJamCluesController(
            CluesService cluesService,
            RequestLogger<LetterJamCluesController> logger
            )
        {
            _cluesService = cluesService;
            _logger = logger;
        }



        [HttpGet]
        [Route("Proposed")]
        public async Task<GenericResponse<IEnumerable<ProposedClueResponse>>> GetProposedCluesAsync([FromQuery]RoundRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _cluesService.GetProposedCluesAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<ProposedClueResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        public async Task<GenericResponseBase> PostAsync(SubmitClueRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _cluesService.SubmitClueAsync(request);

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
        public async Task<GenericResponseBase> DeleteAsync([FromQuery]ClueRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _cluesService.DeleteClueAsync(request);

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
        [Route("GetLettersForClue")]
        public async Task<GenericResponse<IEnumerable<ClueLetterResponse>>> GetLettersForClueAsync([FromQuery]ClueRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _cluesService.GetLettersForClueAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<ClueLetterResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}