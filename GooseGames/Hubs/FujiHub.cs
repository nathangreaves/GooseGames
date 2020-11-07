using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.HubMessages.Fuji;
using Models.Responses.PlayerDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class FujiHub : Hub
    {
        private readonly RequestLogger<FujiHub> _logger;

        public FujiHub(RequestLogger<FujiHub> logger)
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

    public class FujiHubContext
    {
        private readonly IHubContext<FujiHub> _hub;
        private readonly RequestLogger<FujiHubContext> _logger;

        public FujiHubContext(IHubContext<FujiHub> hub, RequestLogger<FujiHubContext> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task SendStartingSessionAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending startingSession: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("startingSession");
        }

        internal async Task SendBeginSessionAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending beginSession: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("beginSession");
        }

        internal async Task SendUpdateSessionAsync(Guid sessionId, FujiUpdate fujiUpdate)
        {
            _logger.LogInformation($"Sending updateSession: to {sessionId}", fujiUpdate);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("updateSession", fujiUpdate);
        }

        internal async Task SendPlayerVictoryAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending playerVictory: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerVictory", playerId);
        }
    }
}

