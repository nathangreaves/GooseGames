using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Responses.LetterJam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.LetterJam
{
    [ApiController]
    [Route("[controller]")]
    public class LetterJamLetterCardController : ControllerBase
    {
        private readonly LetterCardService _letterCardService;
        private readonly RequestLogger<LetterJamLetterCardController> _logger;

        public LetterJamLetterCardController(LetterCardService letterCardService,
            RequestLogger<LetterJamLetterCardController> logger)
        {
            _letterCardService = letterCardService;
            _logger = logger;
        }

        [HttpGet]
        [Route("ReleventLetters")]
        public async Task<GenericResponse<IEnumerable<LetterCardResponse>>> GetReleventLettersAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _letterCardService.GetReleventLettersAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<LetterCardResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
