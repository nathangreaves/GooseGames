using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Requests.JustOne;
using Models.Requests.JustOne.PlayerDetails;
using Models.Responses;
using Models.Responses.JustOne.PlayerDetails;
using RepositoryInterface.JustOne;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class PlayerDetailsService
    {
        private readonly SessionService _sessionService;
        private readonly IPlayerRepository _playerRepository;
        private readonly RequestLogger<PlayerDetailsService> _logger;
        private readonly IHubContext<PlayerHub> _lobbyHub;

        public PlayerDetailsService(SessionService sessionService, IPlayerRepository playerRepository, RequestLogger<PlayerDetailsService> logger, IHubContext<PlayerHub> lobbyHub)
        {
            _sessionService = sessionService;
            _playerRepository = playerRepository;
            _logger = logger;
            _lobbyHub = lobbyHub;
        }

        internal async Task UpdateSignalRConnectionIdAsync(string playerId, string connectionId)
        {
            if (Guid.TryParse(playerId, out Guid id))
            {
                var player = await _playerRepository.GetAsync(id);

                if (player != null)
                {
                    player.ConnectionId = connectionId;

                    await _playerRepository.UpdateAsync(player);
                }
            }
        }

        public async Task<GenericResponse<UpdatePlayerDetailsResponse>> UpdatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
        {
            _logger.LogTrace("Starting update of player details");

            var validationResult = await ValidatePlayerDetailsAsync(request);
            if (validationResult != null && !validationResult.Success)
            {
                return validationResult;
            }

            var player = await _playerRepository.GetAsync(request.PlayerId);
            if (player == null)
            {
                _logger.LogInformation("Unable to find player.");

                return NewResponse.Error<UpdatePlayerDetailsResponse>("Unable to find player.");
            }

            if (player.Name != null)
            {
                _logger.LogInformation($"Attempt to set name of {player.Name} to {request.PlayerName}");
                return NewResponse.Error<UpdatePlayerDetailsResponse>("Unable to set name again.");
            }

            player.Name = request.PlayerName;

            _logger.LogTrace("Getting player number");
            int countOfNamedPlayers = await _playerRepository.CountAsync(p => p.SessionId == request.SessionId && p.Name != null);
            player.PlayerNumber = countOfNamedPlayers + 1;

            _logger.LogTrace("Updating player details");
            await _playerRepository.UpdateAsync(player);

            _logger.LogTrace("Sending update to clients");
            await _lobbyHub.SendPlayerDetailsUpdated(player.SessionId, new PlayerDetailsResponse
            {
                Id = player.Id,
                PlayerName = player.Name,
                PlayerNumber = player.PlayerNumber,
                IsSessionMaster = false
            });

            _logger.LogTrace("Finished updating player details");

            return NewResponse.Ok(new UpdatePlayerDetailsResponse());
        }

        public async Task DeletePlayerAsync(DeletePlayerRequest request)
        {
            _logger.LogTrace("Deleting Player");

            var playerToDelete = await _playerRepository.GetAsync(request.PlayerToDeleteId);

            var isSessionMaster = await _sessionService.ValidateSessionMasterAsync(playerToDelete.SessionId, request.SessionMasterId);
            if (isSessionMaster)
            {
                await _playerRepository.DeleteAsync(playerToDelete);

                await _lobbyHub.SendPlayerRemoved(playerToDelete.SessionId, playerToDelete.Id);
            }

            _logger.LogTrace("Deleted Player");
        }

        public async Task<GenericResponse<GetPlayerDetailsResponse>> GetPlayerDetailsAsync(GetPlayerDetailsRequest request)
        {
            _logger.LogTrace("Starting fetch of player details");

            _logger.LogTrace("Fetchomg session");
            var session = await _sessionService.GetSessionAsync(request.SessionId);
            if (session == null)
            {
                _logger.LogInformation("Unable to find session.");
                return NewResponse.Error<GetPlayerDetailsResponse>("Unable to find session.");
            }

            var players = await _playerRepository.FilterAsync(player => player.SessionId == request.SessionId);
            if (!players.Any(p => p.Id == request.PlayerId))
            {
                _logger.LogInformation("Player did not exist on session");
                return NewResponse.Error<GetPlayerDetailsResponse>("Player does not exist on session");
            }

            var masterPlayerId = session.SessionMasterId;

            var response = new GetPlayerDetailsResponse
            {
                SessionMaster = request.PlayerId == masterPlayerId,
                Players = players.OrderBy(p => p.PlayerNumber == 0 ? int.MaxValue : p.PlayerNumber).Select(p => new PlayerDetailsResponse 
                {
                    Id = p.Id,
                    IsSessionMaster = p.Id == masterPlayerId,
                    PlayerName = p.Name,
                    PlayerNumber = p.PlayerNumber
                })
            };

            return NewResponse.Ok(response);
        }

        private async Task<GenericResponse<UpdatePlayerDetailsResponse>> ValidatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
        {
            _logger.LogTrace("Starting validation of player details");

            if (string.IsNullOrWhiteSpace(request.PlayerName))
            {
                _logger.LogInformation("Empty player name provided");
                return NewResponse.Error<UpdatePlayerDetailsResponse>("Please enter your name.");
            }

            if (request.PlayerName.Length > 20)
            {
                _logger.LogInformation("Player name too long");
                return NewResponse.Error<UpdatePlayerDetailsResponse>("Please enter a player name that is 20 characters or fewer");
            }

            if (!(await _sessionService.ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.New)))
            {
                _logger.LogInformation("Unable to find session. Either it is not new or doesn't exist.");

                return NewResponse.Error<UpdatePlayerDetailsResponse>("Unable to find session. Either it started without you or doesn't exist");
            }

            return null;
        }
    }
}
