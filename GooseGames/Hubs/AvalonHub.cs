using Enums.Avalon;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class AvalonHub : Hub
    {
        private readonly RequestLogger<AvalonHub> _logger;

        public AvalonHub(RequestLogger<AvalonHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var playerId = Context.GetHttpContext().Request.Query["playerId"].FirstOrDefault();
            var sessionId = Context.GetHttpContext().Request.Query["sessionId"].FirstOrDefault();
            var connectionId = Context.ConnectionId;

            await Groups.AddToGroupAsync(connectionId, sessionId);

            _logger.LogInformation($"Client Connected: connectionId={connectionId} : playerId={playerId} : sessionId={sessionId}");

            await base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception e)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Client Disconnected: connectionId={connectionId} : e={e}");

            return base.OnDisconnectedAsync(e);
        }

        public async Task PushSelectedRoles(Guid playerId, Guid sessionId, IEnumerable<GameRoleEnum> roles)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("selectedRoles", playerId, roles);
        }

        public async Task RequestSelectedRoles(Guid sessionId)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("requestedSelectedRoles");

        }
        public async Task RequestSelectedRolesAsSessionMaster(Guid sessionId)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("requestedSelectedRolesAsSessionMaster");
        }
    }

    public class AvalonHubContext
    {
        private readonly IHubContext<AvalonHub> _hub;
        private readonly RequestLogger<AvalonHubContext> _logger;

        public AvalonHubContext(IHubContext<AvalonHub> hub, RequestLogger<AvalonHubContext> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task SendBeginSessionAsync(Guid sessionId, Guid gameId)
        {
            _logger.LogInformation($"Sending beginSession: to {sessionId} : gameId={gameId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("beginSession", gameId);
        }

        internal async Task SendWeightAsync(Guid sessionId, int weight)
        {
            _logger.LogInformation($"Sending weight: to {sessionId} : weight={weight}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("weight", weight);
        }
    }
}