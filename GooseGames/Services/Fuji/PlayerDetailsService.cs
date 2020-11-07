//using GooseGames.Hubs;
//using GooseGames.Logging;
//using Models.Requests;
//using Models.Requests.PlayerDetails;
//using Models.Responses;
//using Models.Responses.PlayerDetails;
//using RepositoryInterface.Fuji;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace GooseGames.Services.Fuji
//{
//    public class PlayerDetailsService
//    {
//        private readonly SessionService _sessionService;
//        private readonly IPlayerRepository _playerRepository;
//        private readonly RequestLogger<PlayerDetailsService> _logger;
//        private readonly FujiHubContext _fujiHubContext;

//        public PlayerDetailsService(SessionService sessionService, 
//            IPlayerRepository playerRepository, 
//            RequestLogger<PlayerDetailsService> logger, 
//            FujiHubContext fujiHubContext)
//        {
//            _sessionService = sessionService;
//            _playerRepository = playerRepository;
//            _logger = logger;
//            _fujiHubContext = fujiHubContext;
//        }


//        //public async Task<GenericResponse<GetPlayerDetailsResponse>> GetPlayerDetailsAsync(PlayerSessionRequest request)
//        //{
//        //    _logger.LogTrace("Starting fetch of player details");

//        //    _logger.LogTrace("Fetching session");
//        //    var session = await _sessionService.GetSessionAsync(request.SessionId);
//        //    if (session == null)
//        //    {
//        //        _logger.LogWarning("Unable to find session.");
//        //        return GenericResponse<GetPlayerDetailsResponse>.Error("Unable to find session.");
//        //    }

//        //    var players = await _playerRepository.FilterAsync(player => player.SessionId == request.SessionId);
//        //    if (!players.Any(p => p.Id == request.PlayerId))
//        //    {
//        //        _logger.LogWarning("Player did not exist on session");
//        //        return GenericResponse<GetPlayerDetailsResponse>.Error("Player does not exist on session");
//        //    }

//        //    var masterPlayerId = session.SessionMasterId;

//        //    var sessionMaster = players.FirstOrDefault(p => p.Id == masterPlayerId);

//        //    var response = new GetPlayerDetailsResponse
//        //    {
//        //        SessionMaster = request.PlayerId == masterPlayerId,
//        //        SessionMasterName = sessionMaster?.Name,
//        //        SessionMasterPlayerNumber = sessionMaster?.PlayerNumber,
//        //        Password = session.Password,
//        //        Players = players.OrderBy(p => p.PlayerNumber == 0 ? int.MaxValue : p.PlayerNumber).Select(p => new PlayerDetailsResponse
//        //        {
//        //            Id = p.Id,
//        //            IsSessionMaster = p.Id == masterPlayerId,
//        //            PlayerName = p.Name,
//        //            PlayerNumber = p.PlayerNumber
//        //        })
//        //    };

//        //    return GenericResponse<GetPlayerDetailsResponse>.Ok(response);
//        //}


//        //internal async Task<GenericResponseBase> UpdatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
//        //{
//        //    _logger.LogTrace("Starting update of player details");

//        //    var validationResult = await ValidatePlayerDetailsAsync(request);
//        //    if (validationResult != null && !validationResult.Success)
//        //    {
//        //        return validationResult;
//        //    }

//        //    var player = await _playerRepository.GetAsync(request.PlayerId);
//        //    if (player == null)
//        //    {
//        //        _logger.LogWarning("Unable to find player.");

//        //        return GenericResponseBase.Error("Unable to find player.");
//        //    }

//        //    if (player.Name != null)
//        //    {
//        //        _logger.LogWarning($"Attempt to set name of {player.Name} to {request.PlayerName}");
//        //        return GenericResponseBase.Error("Unable to set name again.");
//        //    }

//        //    player.Name = request.PlayerName;

//        //    if (player.PlayerNumber == 0)
//        //    {
//        //        _logger.LogTrace("Getting player number");
//        //        int nextPlayerNumber = await _playerRepository.GetNextPlayerNumberAsync(request.SessionId);
//        //        _logger.LogTrace($"Player number = {nextPlayerNumber}");
//        //        player.PlayerNumber = nextPlayerNumber;
//        //    }

//        //    _logger.LogTrace("Updating player details");
//        //    await _playerRepository.UpdateAsync(player);

//        //    _logger.LogTrace("Sending update to clients");
//        //    await _fujiHubContext.SendPlayerDetailsUpdated(player.SessionId, new PlayerDetailsResponse
//        //    {
//        //        Id = player.Id,
//        //        PlayerName = player.Name,
//        //        PlayerNumber = player.PlayerNumber,
//        //        IsSessionMaster = false
//        //    });

//        //    _logger.LogTrace("Finished updating player details");

//        //    return GenericResponseBase.Ok();
//        //}

//        //internal async Task<GenericResponseBase> DeletePlayerAsync(DeletePlayerRequest request)
//        //{
//        //    _logger.LogTrace("Deleting Player", request);

//        //    var requestingPlayer = await _playerRepository.GetAsync(request.SessionMasterId);
//        //    if (requestingPlayer == null)
//        //    {
//        //        return GenericResponseBase.Error("Who even are you?");
//        //    }

//        //    var playerToDelete = await _playerRepository.GetAsync(request.PlayerToDeleteId);
//        //    if (playerToDelete == null)
//        //    {
//        //        _logger.LogWarning("Asked to delete player that didn't exist");
//        //        await _fujiHubContext.SendPlayerRemoved(requestingPlayer.SessionId, request.PlayerToDeleteId);
//        //        return GenericResponseBase.Ok();
//        //    }

//        //    var isSessionMaster = await _sessionService.ValidateSessionMasterAsync(playerToDelete.SessionId, request.SessionMasterId);
//        //    if (isSessionMaster)
//        //    {
//        //        await _playerRepository.DeleteAsync(playerToDelete);

//        //        await _fujiHubContext.SendPlayerRemoved(playerToDelete.SessionId, playerToDelete.Id);
//        //    }

//        //    _logger.LogTrace("Deleted Player");
//        //    return GenericResponseBase.Ok();
//        //}


//        //private async Task<GenericResponseBase> ValidatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
//        //{
//        //    _logger.LogTrace("Starting validation of player details");

//        //    if (string.IsNullOrWhiteSpace(request.PlayerName))
//        //    {
//        //        _logger.LogWarning("Empty player name provided");
//        //        return GenericResponseBase.Error("Please enter your name.");
//        //    }

//        //    if (request.PlayerName.Length > 20)
//        //    {
//        //        _logger.LogWarning("Player name too long");
//        //        return GenericResponseBase.Error("Please enter a player name that is 20 characters or fewer");
//        //    }

//        //    if (!(await _sessionService.ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.New)))
//        //    {
//        //        _logger.LogWarning("Unable to find session. Either it is not new or doesn't exist.");

//        //        return GenericResponseBase.Error("Unable to find session. Either it started without you or doesn't exist");
//        //    }

//        //    return null;
//        //}
//    }
//}
