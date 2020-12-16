using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using Models.Responses.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.LetterJam
{
    [Route("[controller]")]
    [ApiController]
    public class LetterJamFinalWordController : ControllerBase
    {
        private readonly FinalWordService _finalWordService;
        private readonly RequestLogger<LetterJamFinalWordController> _logger;

        public LetterJamFinalWordController(FinalWordService finalWordService,
            RequestLogger<LetterJamFinalWordController> logger)
        {
            _finalWordService = finalWordService;
            _logger = logger;
        }

        [HttpGet]
        [Route("PublicLetters")]
        public async Task<GenericResponse<IEnumerable<FinalWordPublicCardResponse>>> GetFinalWordBonusLettersAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _finalWordService.GetFinalWordBonusCardsAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<FinalWordPublicCardResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        public async Task<GenericResponse<IEnumerable<FinalWordLetterCardResponse>>> GetFinalWordAsync([FromQuery]PlayerSessionGameRequest request)
        {

            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _finalWordService.GetFinalWordAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<FinalWordLetterCardResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        public async Task<GenericResponseBase> SubmitFinalWordAsync(FinalWordRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _finalWordService.PostAsync(request);

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
