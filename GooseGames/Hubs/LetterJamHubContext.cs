using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class LetterJamHub : Hub
    {
        private readonly RequestLogger<LetterJamHub> _logger;

        public LetterJamHub(RequestLogger<LetterJamHub> logger)
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

    public class LetterJamHubContext
    {
        private readonly IHubContext<LetterJamHub> _hub;
        private readonly RequestLogger<LetterJamHubContext> _logger;

        public LetterJamHubContext(IHubContext<LetterJamHub> hub, RequestLogger<LetterJamHubContext> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public async Task SendStartingSessionAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending startingSession: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("startingSession");
        }
        public async Task SendBeginSessionAsync(Guid sessionId, Guid gameId)
        {
            _logger.LogInformation($"Sending beginSession: to {sessionId} : gameId={gameId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("beginSession", gameId);
        }
        public async Task SendPlayerHasChosenStartingWordAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending playerHasChosenStartingWord: to {sessionId} : playerId={playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerHasChosenStartingWord", playerId);
        }
        public async Task SendBeginNewRoundAsync(Guid sessionId, Guid roundId)
        {
            _logger.LogInformation($"Sending beginNewRound: to {sessionId} : roundId={roundId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("beginNewRound", roundId);
        }
    }
}
