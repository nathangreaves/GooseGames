using GooseGames.Logging;
using GooseGames.Services.Werewords;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Werewords
{
    [ApiController]
    [Route("[controller]")]
    public class WerewordsLobbyController : ControllerBase
    {
        private readonly LobbyService _lobbyService;
        private readonly RequestLogger<WerewordsLobbyController> _logger;

        public WerewordsLobbyController(
                LobbyService lobbyService,
                RequestLogger<WerewordsLobbyController> logger
            )
        {
            _lobbyService = lobbyService;
            _logger = logger;
        }

        [HttpPost]
        [Route("StartSession")]
        public async Task<GenericResponseBase> StartSessionAsync(PlayerSessionRequest request)
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
