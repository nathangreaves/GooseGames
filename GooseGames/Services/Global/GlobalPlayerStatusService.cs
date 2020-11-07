using Entities.Codenames;
using Entities.Global.Enums;
using GooseGames.Logging;
using Models.Requests;
using Models.Responses;
using Models.Responses.PlayerStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.Global
{
    public class GlobalPlayerStatusService
    {
        private readonly SessionService _sessionService;
        private readonly PlayerService _playerService;
        private readonly RequestLogger<GlobalPlayerStatusService> _logger;

        public GlobalPlayerStatusService(
            SessionService sessionService,
            PlayerService playerService,
            RequestLogger<GlobalPlayerStatusService> logger
            )
        {
            _sessionService = sessionService;
            _playerService = playerService;
            _logger = logger;
        }

        internal async Task<GenericResponse<PlayerStatusEnum>> ValidatePlayerStatusAsync(PlayerSessionRequest request, GameEnum game) 
        {
            var player = await _playerService.GetAsync(request.PlayerId);
            if (player == null)
            {
                return GenericResponse<PlayerStatusEnum>.Error("a530d7fa-f842-492b-a0fc-6473af1c907a");
            }
            if (player.SessionId == null)
            {
                return GenericResponse<PlayerStatusEnum>.Error("511c0fb3-7d49-4fdf-a1a7-b1281b5ada4b");
            }

            var session = await _sessionService.GetAsync(request.SessionId);
            if (session == null)
            {
                return GenericResponse<PlayerStatusEnum>.Error("You are not part of this session");
            }

            if (session.Status == SessionStatusEnum.Abandoned)
            {
                return GenericResponse<PlayerStatusEnum>.Error("This session was abandoned");
            }

            if (session.Game.HasValue && session.Game.Value != game)
            {
                return GenericResponse<PlayerStatusEnum>.Error("This session is in progress with a different game");
            }

            return GenericResponse<PlayerStatusEnum>.Ok(player.Status);
        }
    }
}
