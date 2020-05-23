using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.Requests;
using Models.Requests.PlayerDetails;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.PlayerDetails;
using Models.Responses.Sessions;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GooseGames.Services.Werewords
{
    public class SessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;        
        private readonly RoundService _roundService;
        private readonly PlayerStatusService _playerStatusService;
        private readonly RequestLogger<SessionService> _logger;
        private readonly WerewordsHubContext _werewordsHubContext;
        private const int MinNumberOfPlayersPerSession = 4;
        private const int MaxNumberOfPlayersPerSession = 10;

        public SessionService(ISessionRepository sessionRepository, 
            IPlayerRepository playerRepository, 
            RoundService roundService,
            PlayerStatusService playerStatusService,
            RequestLogger<SessionService> logger,
            WerewordsHubContext lobbyHub)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _roundService = roundService;
            _playerStatusService = playerStatusService;
            _logger = logger;
            _werewordsHubContext = lobbyHub;
        }

        public async Task<GenericResponse<JoinSessionResponse>> CreateOrJoinSessionAsync(JoinSessionRequest request)
        {
            _logger.LogTrace($"Starting session creation");

            var password = request.Password;

            await CleanUpExpiredSessionsForPassword(request.Password);

            if (await SessionExistsForPasswordAsync(password))
            {
                _logger.LogTrace("Session already exists");
                return await JoinSessionAsync(request);
            }
            var dateTime = DateTime.UtcNow;

            Player newPlayer = new Player
            {
                CreatedUtc = dateTime,
                Status = PlayerStatusEnum.New
            };
            Session newSession = new Session
            {
                CreatedUtc = dateTime,
                Password = password,
                Players = new List<Player>
                {
                    newPlayer
                },
                LastUpdatedUtc = dateTime
            };

            try
            {
                _logger.LogTrace($"Ok to insert session");
                return await InsertSessionAsync(newPlayer, newSession);
            }
            catch (Exception firstException)
            {
                _sessionRepository.Detach(newSession);
                _logger.LogError("Failed on first attempt inserting session", firstException);                
                if (await SessionExistsForPasswordAsync(password))
                {
                    _logger.LogTrace("Session already exists");
                    return await JoinSessionAsync(request);
                }
                throw;
            }
        }

        private async Task<GenericResponse<JoinSessionResponse>> InsertSessionAsync(Player newPlayer, Session newSession)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                _logger.LogTrace($"Inserting session");
                                
                await _sessionRepository.InsertAsync(newSession);
                newSession.SessionMaster = newPlayer;
                _logger.LogTrace($"Setting session master");
                await _sessionRepository.UpdateAsync(newSession);

                transaction.Complete();                
            }                       

            _logger.LogTrace($"Session inserted");
            return GenericResponse<JoinSessionResponse>.Ok(new JoinSessionResponse
            {
                SessionId = newSession.Id,
                PlayerId = newPlayer.Id
            });
        }

        internal async Task<GenericResponse<bool>> StartSessionAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Starting session", request);

            if (!await ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.New))
            {
                _logger.LogWarning("Session did not exist to start");
                return GenericResponse<bool>.Error("Session does not exist");
            }
            if (!await ValidateSessionMasterAsync(request.SessionId, request.PlayerId))
            {
                _logger.LogWarning("Request to start session from player other than the session master");
                return GenericResponse<bool>.Error("You do not have the authority to start the session");
            }
            if (!await ValidateMinimumNumberOfPlayersAsync(request.SessionId))
            {
                _logger.LogWarning("Request to start session with not enough players");
                return GenericResponse<bool>.Error("There are not yet enough players to start");
            }
            _logger.LogTrace("Session cleared to start");

            _logger.LogTrace("Fetching session");
            var session = await _sessionRepository.GetAsync(request.SessionId);
            session.StatusId = SessionStatusEnum.InProgress;

            _logger.LogTrace("Marking session in progress");
            await _sessionRepository.UpdateAsync(session);

            _logger.LogTrace("Removing unready players");
            await _playerRepository.DeleteUnreadyPlayersAsync(request.SessionId);

            _logger.LogTrace("Updating all players to RoundWaiting status");
            await _playerStatusService.UpdateAllPlayersForSessionAsync(request.SessionId, PlayerStatusEnum.RoundWaiting);            

            _logger.LogTrace("Sending update to clients");
            await _werewordsHubContext.SendStartingSessionAsync(request.SessionId);

            await Task.Delay(TimeSpan.FromSeconds(2));

            await CleanUpExpiredSessions(request.SessionId);

            await _roundService.CreateNewRoundAsync(session);

            return GenericResponse<bool>.Ok(true);
        }

        internal async Task<GenericResponseBase> UpdatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
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
                _logger.LogWarning("Unable to find player.");

                return GenericResponseBase.Error("Unable to find player.");
            }

            player.Name = request.PlayerName;

            if (player.PlayerNumber == 0)
            {
                _logger.LogTrace("Getting player number");
                int nextPlayerNumber = await _playerRepository.GetNextPlayerNumberAsync(request.SessionId);
                _logger.LogTrace($"Player number = {nextPlayerNumber}");
                player.PlayerNumber = nextPlayerNumber;
            }

            player.Status = PlayerStatusEnum.InLobby;

            _logger.LogTrace("Updating player details");
            await _playerRepository.UpdateAsync(player);

            _logger.LogTrace("Sending update to clients");
            await _werewordsHubContext.SendPlayerDetailsUpdated(player.SessionId.Value, new PlayerDetailsResponse
            {
                Id = player.Id,
                PlayerName = player.Name,
                PlayerNumber = player.PlayerNumber,
                IsSessionMaster = false,
                Ready = true
            });

            _logger.LogTrace("Finished updating player details");

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> AgainAsync(PlayerSessionRequest request)
        {
            var player = await _playerRepository.GetAsync(request.PlayerId);
            if (player == null)
            {
                return GenericResponseBase.Error($"Player not found");
            }

            player.Status = PlayerStatusEnum.InLobby;

            await _playerRepository.UpdateAsync(player);

            await _werewordsHubContext.SendPlayerDetailsUpdated(player.SessionId.Value, new PlayerDetailsResponse
            {
                Id = player.Id,
                PlayerName = player.Name,
                PlayerNumber = player.PlayerNumber,
                Ready = true
            });

            return GenericResponseBase.Ok();
        }

        private async Task CleanUpExpiredSessionsForPassword(string password)
        {
            await _sessionRepository.AbandonSessionsOlderThanAsync(password, DateTime.UtcNow.AddMinutes(-30));
        }

        private async Task CleanUpExpiredSessions(Guid sessionId)
        {
            await _sessionRepository.AbandonSessionsOlderThanAsync(sessionId, DateTime.UtcNow.AddDays(-1));
        }

        private async Task<bool> ValidateMinimumNumberOfPlayersAsync(Guid sessionId)
        {
            var readyPlayers = await _playerRepository.CountAsync(p => p.SessionId == sessionId && p.Name != null && p.PlayerNumber > 0);

            return readyPlayers >= MinNumberOfPlayersPerSession;
        }

        internal async Task<GenericResponse<JoinSessionResponse>> JoinSessionAsync(JoinSessionRequest request)
        {
            _logger.LogTrace($"Starting session join");
            var password = request.Password;

            var session = await GetSessionFromPasswordAsync(password);

            if (session == null)
            {
                _logger.LogWarning("Session doesn't exist");
                return NewResponse.Error<JoinSessionResponse>($"Session doesn't exist with identifier: {password}");
            }
            if (session.StatusId != SessionStatusEnum.New)
            {
                _logger.LogWarning("Player tried to join an in progress session");
                return GenericResponse<JoinSessionResponse>.Error("Session is already in progress");
            }

            _logger.LogTrace($"Getting count of players");
            var countOfPlayers = await _playerRepository.CountAsync(p => p.SessionId == session.Id);
            if (countOfPlayers >= MaxNumberOfPlayersPerSession)
            {
                _logger.LogDebug($"Already {countOfPlayers} on session");
                return NewResponse.Error<JoinSessionResponse>($"Session is full");
            }

            _logger.LogTrace($"Ok to insert player");

            Player newPlayer = new Player();          
            newPlayer.SessionId = session.Id;
            newPlayer.Status = PlayerStatusEnum.New;

            await _playerRepository.InsertAsync(newPlayer);

            _logger.LogTrace($"Player inserted");

            _logger.LogTrace("Sending update to clients");
            await _werewordsHubContext.SendPlayerAdded(newPlayer.SessionId.Value, new PlayerDetailsResponse
            {
                Id = newPlayer.Id
            });

            return NewResponse.Ok(new JoinSessionResponse 
            {
                SessionId = session.Id,
                PlayerId = newPlayer.Id
            });
        }


        public async Task<GenericResponse<GetPlayerDetailsResponse>> GetPlayerDetailsAsync(PlayerSessionRequest request)
        {
            _logger.LogTrace("Starting fetch of player details");

            _logger.LogTrace("Fetching session");
            var session = await GetSessionAsync(request.SessionId);
            if (session == null)
            {
                _logger.LogWarning("Unable to find session.");
                return GenericResponse<GetPlayerDetailsResponse>.Error("Unable to find session.");
            }

            var players = await _playerRepository.FilterAsync(player => player.SessionId == request.SessionId);
            if (!players.Any(p => p.Id == request.PlayerId))
            {
                _logger.LogWarning("Player did not exist on session");
                return GenericResponse<GetPlayerDetailsResponse>.Error("Player does not exist on session");
            }

            var masterPlayerId = session.SessionMasterId;

            var sessionMaster = players.FirstOrDefault(p => p.Id == masterPlayerId);

            var response = new GetPlayerDetailsResponse
            {
                SessionMaster = request.PlayerId == masterPlayerId,
                SessionMasterName = sessionMaster?.Name,
                SessionMasterPlayerNumber = sessionMaster?.PlayerNumber,
                Password = session.Password,
                Players = players.OrderBy(p => p.PlayerNumber == 0 ? int.MaxValue : p.PlayerNumber).Select(p => new PlayerDetailsResponse
                {
                    Id = p.Id,
                    IsSessionMaster = p.Id == masterPlayerId,
                    PlayerName = p.Name,
                    PlayerNumber = p.PlayerNumber,
                    Ready = p.Status == PlayerStatusEnum.InLobby
                })
            };

            return GenericResponse<GetPlayerDetailsResponse>.Ok(response);
        }
        internal async Task<GenericResponseBase> DeletePlayerAsync(DeletePlayerRequest request)
        {
            _logger.LogTrace("Deleting Player", request);

            var requestingPlayer = await _playerRepository.GetAsync(request.SessionMasterId);
            if (requestingPlayer == null)
            {
                return GenericResponseBase.Error("Who even are you?");
            }

            var playerToDelete = await _playerRepository.GetAsync(request.PlayerToDeleteId);
            if (playerToDelete == null)
            {
                _logger.LogWarning("Asked to delete player that didn't exist");
                await _werewordsHubContext.SendPlayerRemoved(requestingPlayer.SessionId.Value, request.PlayerToDeleteId);
                return GenericResponseBase.Ok();
            }

            var isSessionMaster = await ValidateSessionMasterAsync(playerToDelete.SessionId.Value, request.SessionMasterId);
            if (isSessionMaster)
            {
                playerToDelete.SessionId = null;
                await _playerRepository.UpdateAsync(playerToDelete);

                await _werewordsHubContext.SendPlayerRemoved(requestingPlayer.SessionId.Value, playerToDelete.Id);
            }

            _logger.LogTrace("Deleted Player");
            return GenericResponseBase.Ok();
        }

        private async Task<bool> SessionExistsForPasswordAsync(string password)
        {
            _logger.LogTrace($"Checking existance of session with password {password}");
            var found = await GetSessionFromPasswordAsync(password);

            return found != null;
        }

        private async Task<Session> GetSessionFromPasswordAsync(string password)
        {
            _logger.LogTrace($"Fetching session with password {password}");

            return await _sessionRepository.SingleOrDefaultAsync(session => 
                (session.StatusId == SessionStatusEnum.New 
                || session.StatusId == SessionStatusEnum.InProgress) 
                && session.Password.ToLower() == password.ToLower());
        }

        public async Task<bool> ValidateSessionStatusAsync(Guid sessionId, SessionStatusEnum status) 
        {
            _logger.LogTrace("Validating session exists and has status: ", status);

            return await _sessionRepository.SingleResultMatchesAsync(sessionId, s => s.StatusId == status);
        }
        internal async Task<bool> ValidateSessionMasterAsync(Guid sessionId, Guid sessionMasterId)
        {
            _logger.LogTrace("Validating session master: ", sessionMasterId);

            return await _sessionRepository.SingleResultMatchesAsync(sessionId, s => s.SessionMasterId == sessionMasterId);
        }
        public async Task<Session> GetSessionAsync(Guid sessionId)
        {
            _logger.LogTrace("Validating session exists");

            return await _sessionRepository.GetAsync(sessionId);
        }
        private async Task<GenericResponseBase> ValidatePlayerDetailsAsync(UpdatePlayerDetailsRequest request)
        {
            _logger.LogTrace("Starting validation of player details");

            if (string.IsNullOrWhiteSpace(request.PlayerName))
            {
                _logger.LogWarning("Empty player name provided");
                return GenericResponseBase.Error("Please enter your name.");
            }

            if (request.PlayerName.Length > 20)
            {
                _logger.LogWarning("Player name too long");
                return GenericResponseBase.Error("Please enter a player name that is 20 characters or fewer");
            }

            if (!(await ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.New)))
            {
                _logger.LogWarning("Unable to find session. Either it is not new or doesn't exist.");

                return GenericResponseBase.Error("Unable to find session. Either it started without you or doesn't exist");
            }

            return null;
        }




        internal async Task<IEnumerable<JoinSessionResponse>> CreateTestSessionAsync(JoinSessionRequest request)
        {
            var existingSession = await GetSessionFromPasswordAsync(request.Password);
            if (existingSession != null)
            {
                var players = (await _playerRepository.FilterAsync(p => p.SessionId == existingSession.Id)).OrderBy(s => s.PlayerNumber);

                return players.Select(p => new JoinSessionResponse
                {
                    PlayerId = p.Id,
                    SessionId = existingSession.Id
                });
            }

            var sessionResponse = await CreateOrJoinSessionAsync(request);
            if (!sessionResponse.Success)
            {
                throw new Exception(sessionResponse.ErrorCode);
            }
            var masterPlayerId = sessionResponse.Data.PlayerId;
            var sessionId = sessionResponse.Data.SessionId;

            var masterPlayer = await _playerRepository.GetAsync(masterPlayerId);
            masterPlayer.Name = "Player 1";
            masterPlayer.PlayerNumber = 1;
            await _playerRepository.UpdateAsync(masterPlayer);

            var player2 = new Player
            {
                SessionId = sessionId,
                Name = "Player 2",
                PlayerNumber = 2
            };

            var player3 = new Player
            {
                SessionId = sessionId,
                Name = "Player 3",
                PlayerNumber = 3
            };

            var player4 = new Player
            {
                SessionId = sessionId,
                Name = "Player 4",
                PlayerNumber = 4
            };

            await _playerRepository.InsertAsync(player2);
            await _playerRepository.InsertAsync(player3);
            await _playerRepository.InsertAsync(player4);

            var session = await _sessionRepository.GetAsync(sessionId);
            session.StatusId = SessionStatusEnum.InProgress;

            await _sessionRepository.UpdateAsync(session);

            _logger.LogTrace("Updating all players to RoundWaiting status");
            await _playerStatusService.UpdateAllPlayersForSessionAsync(session.Id, PlayerStatusEnum.RoundWaiting);

            await _roundService.CreateNewRoundAsync(session);

            return new[]
            {
                new JoinSessionResponse { PlayerId = masterPlayer.Id, SessionId = sessionId } ,

                new JoinSessionResponse { PlayerId = player2.Id, SessionId = sessionId },

                new JoinSessionResponse { PlayerId = player3.Id, SessionId = sessionId },

                new JoinSessionResponse { PlayerId = player4.Id, SessionId = sessionId }
            };
        }

    }
}
