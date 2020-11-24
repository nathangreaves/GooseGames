using Entities.Global;
using Entities.Global.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.Requests;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.PlayerDetails;
using Models.Responses.Sessions;
using RepositoryInterface.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GooseGames.Services.Global
{
    public class SessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly GlobalHubContext _globalHubContext;
        private readonly RequestLogger<SessionService> _logger;

        public SessionService(
            ISessionRepository sessionRepository, 
            IPlayerRepository playerRepository,
            GlobalHubContext globalHubContext,
            RequestLogger<SessionService> logger
            )
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _globalHubContext = globalHubContext;
            _logger = logger;
        }

        internal async Task<Session> GetAsync(Guid sessionId)
        {
            return await _sessionRepository.GetAsync(sessionId);
        }

        internal async Task<Guid?> GetGameIdAsync(Guid sessionId, GameEnum game)
        {
            var session = await _sessionRepository.GetAsync(sessionId);

            return session != null && session.Game == game ? session.GameSessionId : (Guid?)null;
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

            await CleanUpExpiredSessions();

            var dateTime = DateTime.UtcNow;

            Player newPlayer = new Player
            {
                CreatedUtc = dateTime
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

        public async Task<GenericResponse<JoinSessionResponse>> JoinSessionAsync(JoinSessionRequest request)
        {
            _logger.LogTrace($"Starting session join");
            var password = request.Password;

            var session = await GetSessionFromPasswordAsync(password);

            if (session == null)
            {
                _logger.LogWarning("Session doesn't exist");
                return GenericResponse<JoinSessionResponse>.Error($"Session doesn't exist with identifier: {password}");
            }
            if (session.Status != SessionStatusEnum.Lobby)
            {
                _logger.LogWarning("Player tried to join an in progress session");
                return GenericResponse<JoinSessionResponse>.Error("Session is already in progress");
            }

            _logger.LogTrace($"Ok to insert player");

            Player newPlayer = new Player();
            newPlayer.SessionId = session.Id;

            await _playerRepository.InsertAsync(newPlayer);

            _logger.LogTrace($"Player inserted");

            _logger.LogTrace("Sending update to clients");
            await _globalHubContext.SendPlayerAdded(newPlayer.SessionId.Value, new PlayerDetailsResponse
            {
                Id = newPlayer.Id
            });

            return GenericResponse<JoinSessionResponse>.Ok(new JoinSessionResponse
            {
                SessionId = session.Id,
                PlayerId = newPlayer.Id
            });
        }

        internal async Task<GenericResponseBase> ValidateSessionToStartAsync(PlayerSessionRequest request, int minNumberOfPlayersPerSession, int maxNumberOfPlayersPerSession)
        {
            if (!await ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.Lobby))
            {
                _logger.LogWarning("Session did not exist to start");
                return GenericResponseBase.Error("Session does not exist");
            }
            if (!await ValidateSessionMasterAsync(request.SessionId, request.PlayerId))
            {
                _logger.LogWarning("Request to start session from player other than the session master");
                return GenericResponseBase.Error("You do not have the authority to start the session");
            }

            var players = await _playerRepository.FilterAsync(p => p.SessionId == request.SessionId);
            if (!players.All(p => p.Status == PlayerStatusEnum.Ready))
            {
                return GenericResponseBase.Error("Not all players are ready");
            }

            if (players.GroupBy(p => p.Emoji).Count() < players.Count)
            {
                return GenericResponseBase.Error("Please ensure all players are using a different emoji");
            }

            if (players.Count < minNumberOfPlayersPerSession)
            {
                return GenericResponseBase.Error("There are not yet enough players to start");
            }
            if (players.Count > maxNumberOfPlayersPerSession)
            {
                return GenericResponseBase.Error("There are too many players to start");
            }

            return GenericResponseBase.Ok();
        }

        internal async Task<GenericResponseBase> StartSessionAsync(Guid sessionId)
        {
            _logger.LogTrace("Fetching session");
            var session = await _sessionRepository.GetAsync(sessionId);
            session.Status = SessionStatusEnum.InProgress;

            _logger.LogTrace("Marking session in progress");
            await _sessionRepository.UpdateAsync(session);

            var unreadyPlayers = await _playerRepository.DeleteUnreadyPlayersAsync(sessionId);

            foreach (var playerId in unreadyPlayers)
            {
                await _globalHubContext.SendPlayerRemoved(sessionId, playerId);
            }

            await UpdateAllPlayersToStatusAsync(sessionId, PlayerStatusEnum.WaitingForGame);

            await _globalHubContext.SendStartingSessionAsync(sessionId);

            return GenericResponseBase.Ok();
        }

        internal async Task UpdateAllPlayersToStatusAsync(Guid sessionId, PlayerStatusEnum playerStatus)
        {
            var allPlayers = await _playerRepository.FilterAsync(p => p.SessionId == sessionId);

            foreach (var p in allPlayers)
            {
                p.Status = playerStatus;
            }
            await _playerRepository.UpdateRangeAsync(allPlayers);
        }

        internal async Task SetGameSessionIdentifierAsync(Guid sessionId, GameEnum game, Guid gameSessionId)
        {
            var session = await _sessionRepository.GetAsync(sessionId);

            session.Game = game;
            session.GameSessionId = gameSessionId;

            await _sessionRepository.UpdateAsync(session);
        }

        public async Task<bool> ValidateSessionStatusAsync(Guid sessionId, SessionStatusEnum status)
        {
            _logger.LogTrace("Validating session exists and has status: ", status);

            return await _sessionRepository.SingleResultMatchesAsync(sessionId, s => s.Status == status);
        }
        public async Task<bool> ValidateSessionMasterAsync(Guid sessionId, Guid sessionMasterId)
        {
            _logger.LogTrace("Validating session master: ", sessionMasterId);

            return await _sessionRepository.SingleResultMatchesAsync(sessionId, s => s.SessionMasterId == sessionMasterId);
        }

        internal async Task SetToLobbyAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetAsync(sessionId);

            session.Status = SessionStatusEnum.Lobby;

            await _sessionRepository.UpdateAsync(session);
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
                (session.Status == SessionStatusEnum.Lobby
                || session.Status == SessionStatusEnum.InProgress)
                && session.Password.ToLower() == password.ToLower());
        }

        private async Task CleanUpExpiredSessionsForPassword(string password)
        {
            _logger.LogTrace($"Cleaning up expired sessions for password {password}");
            await _sessionRepository.AbandonSessionsOlderThanAsync(password, DateTime.UtcNow.AddMinutes(-30));
        }

        private async Task CleanUpExpiredSessions()
        {
            _logger.LogTrace("Cleaning up expired sessions");
            await _sessionRepository.AbandonSessionsOlderThanAsync(DateTime.UtcNow.AddDays(-1));
        }
    }
}
