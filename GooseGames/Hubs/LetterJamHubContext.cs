using Entities.LetterJam;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.HubMessages.LetterJam;
using Models.Requests;
using Models.Requests.LetterJam;
using Models.Responses.LetterJam;
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
        internal async Task SendRemoveVoteAsync(PlayerSessionRequest request, Guid clueId)
        {
            _logger.LogInformation($"Sending removeVote: to {request.SessionId} : playerId={request.PlayerId}, clueId={clueId}");
            await _hub.Clients.Group(request.SessionId.ToString()).SendAsync("removeVote", request.PlayerId, clueId);
        }

        internal async Task SendAddVoteAsync(PlayerSessionRequest request, Guid clueId)
        {
            _logger.LogInformation($"Sending addVote: to {request.SessionId} : playerId={request.PlayerId}, clueId={clueId}");
            await _hub.Clients.Group(request.SessionId.ToString()).SendAsync("addVote", request.PlayerId, clueId);
        }
        internal async Task SendNewClueAsync(Guid sessionId, ProposedClueResponse proposedClue)
        {
            _logger.LogInformation($"Sending newClue: to {sessionId}", proposedClue);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("newClue", proposedClue);
        }


        internal async Task SendClueRemovedAsync(Guid sessionId, Guid clueId)
        {
            _logger.LogInformation($"Sending removeClue: to {sessionId} : clueId={clueId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("removeClue", clueId);
        }

        internal async Task PromptGiveClueAsync(Guid sessionId, Guid cluePlayerId, Guid clueId)
        {
            _logger.LogInformation($"Sending promptGiveClue: to {sessionId} : playerId={cluePlayerId}, clueId={clueId}");
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("promptGiveClue", cluePlayerId, clueId);
        }

        internal async Task GiveClueAsync(ClueRequest request, Guid clueGiverPlayerId)
        {
            _logger.LogInformation($"Sending giveClue: to {request.SessionId} : clueId={request.ClueId}");
            await _hub.Clients.Group(request.SessionId.ToString()).SendAsync("giveClue", request.ClueId, clueGiverPlayerId);
        }

        internal async Task SendTokenUpdate(Guid sessionId, TokenUpdate update)
        {
            _logger.LogInformation($"Sending tokenUpdate: to {sessionId}", update);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("tokenUpdate", update);
        }
        
        internal async Task SendPlayerMovedOnToNextCard(Guid sessionId, Guid playerId, LetterCardResponse letterCardResponse)
        {
            _logger.LogInformation($"Sending playerMovedOnToNextCard: to {sessionId} : playerId: {playerId}", letterCardResponse);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("playerMovedOnToNextCard", playerId, letterCardResponse);
        }

        internal async Task SendNewNpcCardAsync(Guid sessionId, LetterCardResponse letterCardResponse)
        {
            _logger.LogInformation($"Sending newNpcCard: to {sessionId}", letterCardResponse);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("newNpcCard", letterCardResponse);
        }
        internal async Task SendNewBonusCardAsync(Guid sessionId, LetterCardResponse letterCardResponse)
        {
            _logger.LogInformation($"Sending newBonusCard: to {sessionId}", letterCardResponse);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("newBonusCard", letterCardResponse);
        }
        internal async Task SendBonusLetterGuessedAsync(Guid sessionId, BonusLetterGuessMessage message)
        {
            _logger.LogInformation($"Sending bonusLetterGuessed: to {sessionId}", message);
            await _hub.Clients.Group(sessionId.ToString()).SendAsync("bonusLetterGuessed", message);
        }
    }
}
