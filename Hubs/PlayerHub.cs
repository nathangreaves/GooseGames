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

        public PlayerHub(PlayerDetailsService playerDetailsService)
        {
            _playerDetailsService = playerDetailsService;
        }

        public override async Task OnConnectedAsync()
        {
            var playerId = Context.GetHttpContext().Request.Query["playerId"].FirstOrDefault();
            var sessionId = Context.GetHttpContext().Request.Query["sessionId"].FirstOrDefault();
            var connectionId = Context.ConnectionId;

            await _playerDetailsService.UpdateSignalRConnectionIdAsync(playerId, connectionId);

            await Groups.AddToGroupAsync(connectionId, sessionId);

            await base.OnConnectedAsync();
        }
    }

    public static class LobbyExtensions
    {
        public static async Task SendPlayerAdded(this IHubContext<PlayerHub> hub, Guid sessionId, PlayerDetailsResponse playerDetailsResponse)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("playerAdded", playerDetailsResponse);
        }
        public static async Task SendPlayerDetailsUpdated(this IHubContext<PlayerHub> hub, Guid sessionId, PlayerDetailsResponse playerDetailsResponse)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("playerDetailsUpdated", playerDetailsResponse);
        }
        public static async Task SendPlayerRemoved(this IHubContext<PlayerHub> hub, Guid sessionId, Guid playerId)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("playerRemoved", playerId);
        }
        public static async Task SendStartingSessionAsync(this IHubContext<PlayerHub> hub, Guid sessionId)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("startingSession");
        }
    }

    public static class PlayerWaitingExtensions 
    {
        public static async Task SendBeginRoundAsync(this IHubContext<PlayerHub> hub, Guid sessionId, string activePlayerConnectionId)
        {
            await hub.Clients.GroupExcept(sessionId.ToString(), activePlayerConnectionId).SendAsync("beginRoundPassivePlayer");
            await hub.Clients.Client(activePlayerConnectionId).SendAsync("beginRoundActivePlayer");
        }
    }

    public static class RoundExtensions
    {
        public static async Task SendClueSubmittedAsync(this IHubContext<PlayerHub> hub, Guid sessionId, Guid playerId)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("clueSubmitted", playerId);
        }
        public static async Task SendAllCluesSubmittedAsync(this IHubContext<PlayerHub> hub, Guid sessionId)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("allCluesSubmitted");
        }
        public static async Task SendClueVoteSubmittedAsync(this IHubContext<PlayerHub> hub, Guid sessionId, Guid playerId)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("clueVoteSubmitted", playerId);
        }
        public static async Task SendAllClueVotesSubmittedAsync(this IHubContext<PlayerHub> hub, Guid sessionId)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("allClueVotesSubmitted");
        }
    }
}
