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
    public class LetterJamTableController : ControllerBase
    {
        private readonly TableService _tableService;
        private readonly RequestLogger<LetterJamTableController> _logger;

        public LetterJamTableController(TableService tableService,
            RequestLogger<LetterJamTableController> logger)
        {
            _tableService = tableService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<TableResponse>> GetTableAsync(PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _tableService.GetTableAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<TableResponse>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}