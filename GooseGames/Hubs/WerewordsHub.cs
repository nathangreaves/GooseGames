using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Requests;
using Models.Responses;
using Models.Responses.PlayerDetails;
using Models.Responses.Werewords.Player;
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

        internal async Task SendPlayerAwakeAsync(Guid sessionId, PlayerActionResponse playerActionResponse)
        {
            _logger.LogInformation($"Sending playerAwake: to {sessionId}", playerActionResponse);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerAwake", playerActionResponse);
        }

        internal async Task SendDayBeginAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending dayBegin: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("dayBegin");
        }

        internal async Task SendPlayerResponseAsync(Guid sessionId, PlayerResponse playerResponse)
        {
            _logger.LogInformation($"Sending playerResponse: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerResponse", playerResponse);
        }

        internal async Task SendActivePlayerAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending playerResponse: to {sessionId}", playerId);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("activePlayer", playerId);
        }

        internal async Task SendVoteWerewolvesAsync(Guid sessionId, DateTime endTime, string secretWord)
        {
            _logger.LogInformation($"Sending voteWerewolves: to {sessionId}", new { endTime = endTime, secretWord = secretWord });
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("voteWerewolves", endTime, secretWord);
        }

        internal async Task SendVoteSeerAsync(Guid sessionId, DateTime endTime, IEnumerable<Guid> werewolfPlayerIds, string secretWord)
        {
            _logger.LogInformation($"Sending voteSeer: to {sessionId}", new { endTime = endTime, werewolfPlayerIds = werewolfPlayerIds, secretWord = secretWord });
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("voteSeer", endTime, werewolfPlayerIds, secretWord);
        }

        internal async Task SendStartTimerAsync(Guid sessionId, DateTime dateTime)
        {
            _logger.LogInformation($"Sending startTimer: to {sessionId}", dateTime);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("startTimer", dateTime);
        }

        internal async Task SendRoundOutcomeAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending roundOutcome: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("roundOutcome");
        }
    }
}
