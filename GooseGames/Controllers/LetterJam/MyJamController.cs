using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using Models.Responses.LetterJam;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GooseGames.Controllers.LetterJam
{
    [ApiController]
    [Route("[controller]")]
    public class LetterJamMyJamController : Controller
    {
        private readonly MyJamService _myJamService;
        private readonly RequestLogger<LetterJamMyJamController> _logger;

        public LetterJamMyJamController(MyJamService myJamService,
            RequestLogger<LetterJamMyJamController> logger)
        {
            _myJamService = myJamService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<MyJamResponse>> GetAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _myJamService.GetMyJamAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<MyJamResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPost]
        [Route("LetterGuesses")]
        public async Task<GenericResponseBase> PostLetterGuessesAsync(MyJamLetterGuessesRequest request)
        {

            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _myJamService.PostLetterGuessesAsync(request);

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
