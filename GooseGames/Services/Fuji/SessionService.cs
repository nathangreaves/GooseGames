using Entities.Fuji;
using Entities.Fuji.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Models.Requests;
using Models.Requests.Sessions;
using Models.Responses;
using Models.Responses.Fuji;
using Models.Responses.PlayerDetails;
using Models.Responses.Sessions;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GooseGames.Services.Fuji
{
    public class SessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly FujiHubContext _fujiHubContext;
        private readonly RequestLogger<SessionService> _logger;

        private const int MinNumberOfPlayersPerSession = 4;
        private const int MaxNumberOfPlayersPerSession = 8;

        public SessionService(ISessionRepository sessionRepository,
            IPlayerRepository playerRepository,
            FujiHubContext fujiHubContext,
            RequestLogger<SessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _fujiHubContext = fujiHubContext;
            _logger = logger;
        }

        internal async Task<GenericResponse<NewSessionResponse>> CreateSessionAsync(NewSessionRequest request)
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
                CreatedUtc = dateTime
            };
            var newSession = new Session
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
            return GenericResponse<NewSessionResponse>.Ok(new NewSessionResponse
            {
                SessionId = newSession.Id,
                PlayerId = newPlayer.Id
            });
        }

        internal async Task<GenericResponse<JoinSessionResponse>> JoinSessionAsync(JoinSessionRequest request)
        {
            _logger.LogTrace($"Starting session join");
            var password = request.Password;

            var session = await GetSessionFromPasswordAsync(password);

            if (session == null)
            {
                _logger.LogWarning("Session doesn't exist");
                return GenericResponse<JoinSessionResponse>.Error($"Session doesn't exist with identifier: {password}");
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
                return GenericResponse<JoinSessionResponse>.Error($"Session is full");
            }

            _logger.LogTrace($"Ok to insert player");

            Player newPlayer = new Player();
            newPlayer.Session = session;

            await _playerRepository.InsertAsync(newPlayer);

            _logger.LogTrace($"Player inserted");

            _logger.LogTrace("Sending update to clients");
            await _fujiHubContext.SendPlayerAdded(newPlayer.SessionId, new PlayerDetailsResponse
            {
                Id = newPlayer.Id
            });

            return NewResponse.Ok(new JoinSessionResponse
            {
                SessionId = session.Id,
                PlayerId = newPlayer.Id
            });
        }

        internal async Task<GenericResponseBase> StartSessionAsync(PlayerSessionRequest request)
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

            //_logger.LogTrace("Updating all players to RoundWaiting status");
            //await _playerStatusService.UpdateAllPlayersForSessionAsync(request.SessionId, PlayerStatusEnum.RoundWaiting);

            _logger.LogTrace("Sending update to clients");
            await _fujiHubContext.SendStartingSessionAsync(request.SessionId);

            await Task.Delay(TimeSpan.FromSeconds(2));

            await CleanUpExpiredSessions(request.SessionId);

            //TODO: Prepare Deck
            //await _roundService.PrepareRoundsAsync(request.SessionId);

            await _fujiHubContext.SendBeginSessionAsync(request.SessionId);

            return GenericResponse<bool>.Ok(true);
        }

        internal Task<GenericResponse<SessionResponse>> GetAsync(PlayerSessionRequest request)
        {
            throw new NotImplementedException();
        }

        internal async Task<Session> GetSessionAsync(Guid sessionId)
        {
            _logger.LogTrace("Fetching Session");

            return await _sessionRepository.GetAsync(sessionId);
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

        private async Task<bool> ValidateMinimumNumberOfPlayersAsync(Guid sessionId)
        {
            var readyPlayers = await _playerRepository.CountAsync(p => p.SessionId == sessionId && p.Name != null && p.PlayerNumber > 0);

            return readyPlayers >= MinNumberOfPlayersPerSession;
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
        private async Task CleanUpExpiredSessions(Guid sessionId)
        {
            await _sessionRepository.AbandonSessionsOlderThanAsync(sessionId, DateTime.UtcNow.AddDays(-1));
        }
    }
}
