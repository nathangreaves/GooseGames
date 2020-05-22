using Entities.Werewords;
using Entities.Werewords.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.Requests;
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
        private readonly PlayerHubContext _lobbyHub;
        private const int MinNumberOfPlayersPerSession = 4;
        private const int MaxNumberOfPlayersPerSession = 10;

        public SessionService(ISessionRepository sessionRepository, 
            IPlayerRepository playerRepository, 
            RoundService roundService,
            PlayerStatusService playerStatusService,
            RequestLogger<SessionService> logger,
            PlayerHubContext lobbyHub)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _roundService = roundService;
            _playerStatusService = playerStatusService;
            _logger = logger;
            _lobbyHub = lobbyHub;
        }

        public async Task<GenericResponse<NewSessionResponse>> CreateSessionAsync(NewSessionRequest request)
        {
            _logger.LogTrace($"Starting session creation");

            var password = request.Password;

            if (await SessionExistsForPasswordAsync(password))
            {
                _logger.LogTrace("Session already exists");
                return NewResponse.Error<NewSessionResponse>($"Session already exists with identifier: {password}");
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
                }
            };

            _logger.LogTrace($"Ok to insert session");

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
            return NewResponse.Ok(new NewSessionResponse 
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
            await _lobbyHub.SendStartingSessionAsync(request.SessionId);

            await Task.Delay(TimeSpan.FromSeconds(2));

            await CleanUpExpiredSessions(request.SessionId);

            await _roundService.CreateNewRoundAsync(session);

            return GenericResponse<bool>.Ok(true);
        }

        internal async Task<IEnumerable<JoinSessionResponse>> CreateTestSessionAsync(NewSessionRequest request)
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

            var sessionResponse = await CreateSessionAsync(request);
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
            newPlayer.Session = session;
            newPlayer.Status = PlayerStatusEnum.New;

            await _playerRepository.InsertAsync(newPlayer);

            _logger.LogTrace($"Player inserted");

            _logger.LogTrace("Sending update to clients");
            await _lobbyHub.SendPlayerAdded(newPlayer.SessionId, new PlayerDetailsResponse
            {
                Id = newPlayer.Id
            });

            return NewResponse.Ok(new JoinSessionResponse 
            {
                SessionId = session.Id,
                PlayerId = newPlayer.Id
            });
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
    }
}
