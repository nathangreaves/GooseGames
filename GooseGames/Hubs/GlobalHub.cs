using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Responses.PlayerDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class GlobalHub : Hub
    {
        private readonly RequestLogger<GlobalHub> _logger;

        public GlobalHub(RequestLogger<GlobalHub> logger)
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
    }
    public class GlobalHubContext
    {
        private readonly IHubContext<GlobalHub> _hub;
        private readonly RequestLogger<GlobalHubContext> _logger;

        public GlobalHubContext(IHubContext<GlobalHub> hub, RequestLogger<GlobalHubContext> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task SendPlayerAdded(Guid sessionId, PlayerDetailsResponse playerDetailsResponse)
        {
            _logger.LogInformation($"Sending playerAdded: to {sessionId} :: {playerDetailsResponse.Id}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerAdded", playerDetailsResponse);
        }
        public async Task SendPlayerDetailsUpdated(Guid sessionId, PlayerDetailsResponse playerDetailsResponse)
        {
            _logger.LogInformation($"Sending playerDetailsUpdated: to {sessionId} :: {playerDetailsResponse.Id}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerDetailsUpdated", playerDetailsResponse);
        }
        public async Task SendPlayerRemoved(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending playerRemoved: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerRemoved", playerId);
        }

    }
}
