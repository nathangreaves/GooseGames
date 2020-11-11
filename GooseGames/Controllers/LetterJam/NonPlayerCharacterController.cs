using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LetterJamNonPlayerCharacterController : ControllerBase
    {
        private readonly NonPlayerCharacterService _nonPlayerCharacterService;
        private readonly RequestLogger<LetterJamNonPlayerCharacterController> _logger;

        public LetterJamNonPlayerCharacterController(
            NonPlayerCharacterService nonPlayerCharacterService,
            RequestLogger<LetterJamNonPlayerCharacterController> logger
            )
        {
            _nonPlayerCharacterService = nonPlayerCharacterService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<IEnumerable<NonPlayerCharacterResponse>>> GetAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _nonPlayerCharacterService.GetNonPlayerCharactersAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<NonPlayerCharacterResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
