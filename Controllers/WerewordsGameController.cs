using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GooseGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RepositoryInterface.Werewords.Game;

namespace GooseGames.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WerewordsGameController : ControllerBase
    {
        private readonly IWerewordsGameRepository _werewordsGameRepository;

        public WerewordsGameController(IWerewordsGameRepository werewordsGameRepository)
        {
            _werewordsGameRepository = werewordsGameRepository;
        }

        [HttpPost]
        public async Task<CreateWerewordsGameResponse> PostAsync(CreateWerewordsGameRequest request)
        {
            try
            {
                var password = request.Password;

                if (await _werewordsGameRepository.ExistsWithPassword(password))
                {
                    return new CreateWerewordsGameResponse
                    {
                        ErrorMessage = $"Game already exists with password: {password}"
                    };
                }

                var newId = await _werewordsGameRepository.Create(password);

                return new CreateWerewordsGameResponse
                {
                    GameId = newId
                };
            }
            catch (Exception e)
            {
                return new CreateWerewordsGameResponse
                {
                    ErrorMessage = "Unknown Error"
                };
            }
        }

        [HttpPatch]
        public async Task<JoinWerewordsGameResponse> PatchAsync(JoinWerewordsGameRequest request)
        {
            try
            {
                var password = request.Password;

                if (!await _werewordsGameRepository.ExistsWithPassword(password))
                {
                    return new JoinWerewordsGameResponse
                    {
                        ErrorMessage = $"Game doesn't exist with password: {password}"
                    };
                }

                var game = await _werewordsGameRepository.AddPlayer(password);

                return new JoinWerewordsGameResponse
                {
                    GameId = game.Id,
                    NumberOfPlayers = game.NumberOfPlayers
                };
            }
            catch (Exception e)
            {
                return new JoinWerewordsGameResponse
                {
                    ErrorMessage = "Unknown Error"
                };
            }
        }
    }

    public class CreateWerewordsGameRequest 
    {
        public string Password { get; set; }
    }
    public class JoinWerewordsGameRequest
    {
        public string Password { get; set; }
    }
}