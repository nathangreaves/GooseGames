using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Logging;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GooseGames.Services.JustOne
{
    public class SessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly RequestLogger<SessionService> _logger;

        private const int MaxNumberOfPlayersPerSession = 7;

        public SessionService(ISessionRepository sessionRepository, IPlayerRepository playerRepository, RequestLogger<SessionService> logger)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<NewSessionResponse>> CreateSessionAsync(NewSessionRequest request)
        {
            _logger.LogTrace($"Starting session creation");

            var password = request.Password;

            if (await SessionExistsForPasswordAsync(password))
            {
                _logger.LogTrace("Session already exists");
                return NewResponse.Error<NewSessionResponse>($"Session already exists with password: {password}");
            }

            Player newPlayer = new Player();
            Session newSession = new Session
            {
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

        internal async Task<GenericResponse<JoinSessionResponse>> JoinSessionAsync(JoinSessionRequest request)
        {
            _logger.LogTrace($"Starting session join");

            var password = request.Password;

            var session = await GetSessionFromPasswordAsync(password);

            if (session == null)
            {
                _logger.LogTrace("Session doesn't exist");
                return NewResponse.Error<JoinSessionResponse>($"Session doesn't exist with password: {password}");
            }

            _logger.LogTrace($"Getting count of players");
            var countOfPlayers = await _playerRepository.CountAsync(p => p.SessionId == session.Id);
            if (countOfPlayers >= MaxNumberOfPlayersPerSession)
            {
                _logger.LogTrace($"Already {countOfPlayers} on session");
                return NewResponse.Error<JoinSessionResponse>($"Session is full");
            }

            _logger.LogTrace($"Ok to insert player");

            Player newPlayer = new Player();          
            newPlayer.Session = session;

            await _playerRepository.InsertAsync(newPlayer);

            _logger.LogTrace($"Player inserted");

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
            return await _sessionRepository.SingleOrDefaultAsync(session => session.Password.ToLower() == password.ToLower());
        }

        public async Task<bool> ValidateSessionStatusAsync(Guid sessionId, SessionStatusEnum status) 
        {
            _logger.LogTrace("Validating session exists and has status: ", status);

            return await _sessionRepository.SingleResultMatchesAsync(sessionId, s => s.StatusId == status);
        }

        public async Task<Session> GetSessionAsync(Guid sessionId)
        {
            _logger.LogTrace("Validating session exists");

            return await _sessionRepository.GetAsync(sessionId);
        }
    }
}
