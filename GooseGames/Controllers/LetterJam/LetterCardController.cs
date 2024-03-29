﻿using GooseGames.Logging;
using GooseGames.Services.LetterJam;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.LetterJam;
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

        [HttpPost]
        [Route("GetLetters")]
        public async Task<GenericResponse<IEnumerable<LetterCardResponse>>> GetLettersAsync(LetterCardsRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

               var result = await _letterCardService.GetLettersAsync(request);

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

        [HttpGet]
        [Route("RelevantLetters")]
        public async Task<GenericResponse<IEnumerable<LetterCardResponse>>> GetRelevantLettersAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _letterCardService.GetRelevantLettersAsync(request);

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
