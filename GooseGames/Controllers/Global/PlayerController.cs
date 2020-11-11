using GooseGames.Logging;
using GooseGames.Services.Global;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.PlayerDetails;
using Models.Responses;
using Models.Responses.PlayerDetails;
using Models.Responses.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Global
{
    [ApiController]
    [Route("[controller]")]
    public class GlobalPlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly RequestLogger<GlobalPlayerController> _logger;

        public GlobalPlayerController(PlayerService playerService,
            RequestLogger<GlobalPlayerController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GenericResponse<IEnumerable<PlayerResponse>>> GetPlayersAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var players = await _playerService.GetForSessionAsync(request.SessionId);

                if (!players.Any(p => p.Id == request.PlayerId))
                {
                    var response = GenericResponse<IEnumerable<PlayerResponse>>.Error("This player does not exist on this session");

                    _logger.LogInformation("Returned result", response);

                    return response;
                }

                var playerResponses = players.Select(p => new PlayerResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Emoji = p.Emoji,
                    PlayerNumber = p.PlayerNumber
                });
                var result = GenericResponse<IEnumerable<PlayerResponse>>.Ok(playerResponses);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<PlayerResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        [Route("Lobby")]
        public async Task<GenericResponse<GetPlayerDetailsResponse>> GetLobbyAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerService.GetPlayerDetailsAsync(request);

                _logger.LogInformation("Returned result", result);
                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<GetPlayerDetailsResponse>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpPut]
        [Route("Unready")]
        public async Task<GenericResponseBase> UnreadyPlayerAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerService.UnreadyPlayerAsync(request);

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

        [HttpPut]
        public async Task<GenericResponseBase> UpdatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerService.UpdatePlayerDetailsAsync(request);

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

        [HttpDelete]
        public async Task<GenericResponseBase> KickPlayerAsync([FromQuery]DeletePlayerRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerService.DeletePlayerAsync(request);

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

        [HttpPost]
        [Route("Again")]
        public async Task<GenericResponseBase> AgainAsync(PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _playerService.SendPlayerToLobbyAsync(request);

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
