using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Responses.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Werewords
{
    [ApiController]
    [Route("[controller]")]
    public class WerewordsPlayerRoundInformationController : ControllerBase
    {
        private readonly PlayerRoundInformationService _roundService;
        private readonly RequestLogger<WerewordsPlayerRoundInformationController> _logger;

        public WerewordsPlayerRoundInformationController(PlayerRoundInformationService roundService,
            RequestLogger<WerewordsPlayerRoundInformationController> logger)
        {
            _roundService = roundService;
            _logger = logger;
        }

        [HttpGet]
        [Route("SecretRole")]
        public async Task<GenericResponse<PlayerSecretRoleResponse>> GetSecretRoleAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request");

                var result = await _roundService.GetPlayerSecretRoleAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e);
                return GenericResponse<PlayerSecretRoleResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
