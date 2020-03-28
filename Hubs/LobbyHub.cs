using GooseGames.Services.JustOne;
using Microsoft.AspNetCore.SignalR;
using Models.Responses.JustOne.PlayerDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly PlayerDetailsService _playerDetailsService;

        public LobbyHub(PlayerDetailsService playerDetailsService)
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

    public static class LobbyHubExtensions
    {
        public static async Task SendPlayerAdded(this IHubContext<LobbyHub> hub, Guid sessionId, PlayerDetailsResponse playerDetailsResponse)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("playerAdded", playerDetailsResponse);
        }

        public static async Task SendPlayerDetailsUpdated(this IHubContext<LobbyHub> hub, Guid sessionId, PlayerDetailsResponse playerDetailsResponse)
        {
            await hub.Clients.Group(sessionId.ToString()).SendAsync("playerDetailsUpdated", playerDetailsResponse);
        }

    }
}
