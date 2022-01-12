using GooseGames.Logging;
using GooseGames.Services.Avalon;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Requests.Avalon;
using Models.Responses;
using Models.Responses.Avalon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Controllers.Avalon
{
    [ApiController]
    [Route("[controller]")]
    public class AvalonRolesController : ControllerBase
    {
        private readonly RolesService _rolesService;
        private readonly RequestLogger<AvalonRolesController> _logger;

        public AvalonRolesController(RolesService rolesService,
            RequestLogger<AvalonRolesController> logger)
        {
            _rolesService = rolesService;
            _logger = logger;
        }

        [Route("GetAll")]
        public async Task<GenericResponse<IEnumerable<RoleResponse>>> GetAllAsync([FromQuery]PlayerSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _rolesService.AllRolesAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<RoleResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }

        [HttpGet]
        public async Task<GenericResponse<IEnumerable<RoleResponse>>> GetAsync([FromQuery]PlayerSessionGameRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _rolesService.RolesInGameAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<IEnumerable<RoleResponse>>.Error($"Unknown Error {errorGuid}");
            }
        }


        [HttpPost]
        [Route("GetWeight")]
        public async Task<GenericResponse<double>> GetWeightAsync(StartSessionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request", request);

                var result = await _rolesService.GetWeightAsync(request);

                _logger.LogInformation("Returned result", result);

                return result;
            }
            catch (Exception e)
            {
                var errorGuid = Guid.NewGuid();
                _logger.LogError($"Unknown Error {errorGuid}", e, request);
                return GenericResponse<double>.Error($"Unknown Error {errorGuid}");
            }
        }
    }
}
