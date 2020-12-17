using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.LetterJam
{
    [ApiController]
    [Route("[controller]")]
    public class LetterJamLobbyController : ControllerBase
    {
        private readonly LobbyService _lobbyService;
        private readonly RequestLogger<LetterJamLobbyController> _logger;

        public LetterJamLobbyController(
                LobbyService lobbyService,
                RequestLogger<LetterJamLobbyController> logger
            )
        {
            _lobbyService = lobbyService;
            _logger = logger;
        }

        [HttpPost]
        [Route("StartSession")]
        public async Task<GenericResponseBase> StartSessionAsync(StartSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _lobbyService.StartSessionAsync(request);

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
