using GooseGames.Logging;
using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.SignalR;
using Models.Responses.JustOne.PlayerDetails;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class PlayerHub : Hub
    {
        private readonly PlayerDetailsService _playerDetailsService;
        private readonly RequestLogger<PlayerHub> _logger;

        public PlayerHub(PlayerDetailsService playerDetailsService, RequestLogger<PlayerHub> logger)
        {
            _playerDetailsService = playerDetailsService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var playerId = Context.GetHttpContext().Request.Query["playerId"].FirstOrDefault();
            var sessionId = Context.GetHttpContext().Request.Query["sessionId"].FirstOrDefault();
            var connectionId = Context.ConnectionId;

            await _playerDetailsService.UpdateSignalRConnectionIdAsync(playerId, connectionId);

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

    public class PlayerHubContext
    {
        private readonly IHubContext<PlayerHub> _hub;
        private readonly RequestLogger<PlayerHubContext> _logger;

        public PlayerHubContext(IHubContext<PlayerHub> hub, RequestLogger<PlayerHubContext> logger)
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

        public async Task SendBeginRoundAsync(Guid sessionId, string activePlayerConnectionId)
        {
            _logger.LogInformation($"Sending beginRoundPassivePlayer: to {sessionId}");
            await _hub.Clients.GroupExcept(sessionId.ToString(), activePlayerConnectionId).SendAsync("beginRoundPassivePlayer");

            _logger.LogInformation($"Sending beginRoundActivePlayer: to {sessionId}");
            await _hub.Clients.Client(activePlayerConnectionId).SendAsync("beginRoundActivePlayer");
        }

        public async Task SendPlayerReadyForRoundAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending playerReadyForRound: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerReadyForRound", playerId);
        }

        public async Task SendClueSubmittedAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending clueSubmitted: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("clueSubmitted", playerId);
        }

        public async Task SendClueRevokedAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending clueRevoked: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("clueRevoked", playerId);
        }

        public async Task SendAllCluesSubmittedAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending allCluesSubmitted: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("allCluesSubmitted");
        }
        public async Task SendClueVoteSubmittedAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending clueVoteSubmitted: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("clueVoteSubmitted", playerId);
        }
        public async Task SendAllClueVotesSubmittedAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending allClueVotesSubmitted: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("allClueVotesSubmitted");
        }

        public async Task SendClueVoteRevokedAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending clueVoteRevoked: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("clueVoteRevoked", playerId);
        }

        public async Task SendRoundOutcomeAvailableAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending roundOutcomeAvailable: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("roundOutcomeAvailable");
        }
        public async Task SendActivePlayerResponseVoteRequiredAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending activePlayerResponseVoteRequired: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("activePlayerResponseVoteRequired");
        }

        public async Task SendResponseVoteSubmittedAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending responseVoteSubmitted: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("responseVoteSubmitted", playerId);
        }
        public async Task SendResponseVoteRevokedAsync(Guid sessionId, Guid playerId)
        {
            _logger.LogInformation($"Sending responseVoteRevoked: to {sessionId} :: {playerId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("responseVoteRevoked", playerId);
        }

        public async Task SendAllResponseVotesSubmittedAsync(Guid sessionId)
        {
            _logger.LogInformation($"Sending allResponseVotesSubmitted: to {sessionId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("allResponseVotesSubmitted");
        }
    }
}
