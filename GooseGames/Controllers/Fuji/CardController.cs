using GooseGames.Logging;
using GooseGames.Services.Fuji;
using Microsoft.AspNetCore.Mvc;
using Models.Requests.Fuji;
using Models.Responses;
using Models.Responses.Fuji.Cards;
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

        [HttpGet]
        public async Task<GenericResponse<Card>> GetAsync([FromQuery] CardRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _cardService.GetHandCardAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<Card>.Error($"Unknown Error {errorGuid}");
            }
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

        //TODO: Check if redundant
        [HttpDelete]
        public async Task<GenericResponseBase> DeleteAsync([FromQuery]CardRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _cardService.DiscardHandCardAsync(request.CardId);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<Card>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
