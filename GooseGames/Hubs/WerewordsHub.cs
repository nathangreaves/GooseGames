using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Responses.PlayerDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class WerewordsHub : Hub
    {
        private readonly RequestLogger<WerewordsHub> _logger;

        public WerewordsHub(RequestLogger<WerewordsHub> logger)
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

    public class WerewordsHubContext
    {
        private readonly IHubContext<WerewordsHub> _hub;
        private readonly RequestLogger<WerewordsHubContext> _logger;

        public WerewordsHubContext(IHubContext<WerewordsHub> hub, RequestLogger<WerewordsHubContext> logger)
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

        public async Task SendStartingSessionAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending startingSession: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("startingSession");
        }

        internal async Task SendSecretRoleAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending secretRole: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("secretRole");
        }

        internal async Task SendSecretWordAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending secretWord: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("secretWord");
        }
    }
}
