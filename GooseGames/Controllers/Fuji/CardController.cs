using GooseGames.Logging;
using GooseGames.Services.Fuji;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.Fuji;
using Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Fuji
{
    [ApiController]
    [Route("[controller]")]
    public class FujiCardController : ControllerBase
    {
        private readonly CardService _cardService;
        private readonly RequestLogger<FujiCardController> _logger;

        public FujiCardController(CardService cardService,
            RequestLogger<FujiCardController> logger)
        {
            _cardService = cardService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<GenericResponseBase> PostAsync(PlayCardRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _cardService.PlayCardAsync(request);

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
