using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class CodenamesHub : Hub
    {
        private readonly RequestLogger<CodenamesHub> _logger;

        public CodenamesHub(RequestLogger<CodenamesHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var sessionId = Context.GetHttpContext().Request.Query["sessionId"].FirstOrDefault();
            var connectionId = Context.ConnectionId;

            await Groups.AddToGroupAsync(connectionId, sessionId);

            _logger.LogInformation($"Client Connected: connectionId={connectionId} : sessionId={sessionId}");

            await base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception e)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Client Disconnected: connectionId={connectionId} : e={e}");

            return base.OnDisconnectedAsync(e);
        }
    }

    public class CodenamesHubContext
    {
        private readonly IHubContext<CodenamesHub> _hub;
        private readonly RequestLogger<CodenamesHubContext> _logger;

        public CodenamesHubContext(IHubContext<CodenamesHub> hub, RequestLogger<CodenamesHubContext> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task SendRevealWordAsync(Guid sessionId, Guid wordId)
        {
            _logger.LogInformation($"Sending revealWord: to {sessionId} :: {wordId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("revealWord", wordId);
        }

        public async Task SendWordsRefreshedAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending wordsRefreshed: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("wordsRefreshed");
        }

        public async Task SendIsBlueTurnAsync(Guid sessionId, bool isBlueTurn)
        {
            _logger.LogInformation($"Sending isBlueTurn: to {sessionId} :: {isBlueTurn}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("isBlueTurn", isBlueTurn);
        }

        public async Task SendIsBlueVictoryAsync(Guid sessionId, bool isBlueVictory)
        {
            _logger.LogInformation($"Sending isBlueVictory: to {sessionId} :: {isBlueVictory}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("isBlueVictory", isBlueVictory);
        }
    }
}
