using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.LetterJam;
using Models.Responses;

namespace GooseGames.Controllers.LetterJam
{
    [Route("[controller]")]
    [ApiController]
    public class LetterJamClueVoteController : ControllerBase
    {
        private readonly ClueVoteService _clueVoteService;
        private readonly RequestLogger<LetterJamClueVoteController> _logger;

        public LetterJamClueVoteController(ClueVoteService clueVoteService,
            RequestLogger<LetterJamClueVoteController> logger)
        {
            _clueVoteService = clueVoteService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<GenericResponseBase> PostAsync(ClueRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _clueVoteService.PostAsync(request);

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