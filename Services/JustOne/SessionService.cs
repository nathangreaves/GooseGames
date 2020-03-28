using Entities.JustOne;
using Entities.JustOne.Enums;
using GooseGames.Hubs;
using GooseGames.Logging;
using Microsoft.AspNetCore.SignalR;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;
using Models.Responses.JustOne.PlayerDetails;
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
        private readonly PlayerStatusService _playerStatusService;
        private readonly RoundService _roundService;
        private readonly RequestLogger<SessionService> _logger;
        private readonly IHubContext<PlayerHub> _lobbyHub;
        private const int MaxNumberOfPlayersPerSession = 7;

        public SessionService(ISessionRepository sessionRepository, 
            IPlayerRepository playerRepository, 
            PlayerStatusService playerStatusService, 
            RoundService roundService,
            RequestLogger<SessionService> logger, 
            IHubContext<PlayerHub> lobbyHub)
        {
            _sessionRepository = sessionRepository;
            _playerRepository = playerRepository;
            _playerStatusService = playerStatusService;
            _roundService = roundService;
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
                return NewResponse.Error<NewSessionResponse>($"Session already exists with password: {password}");
            }

            Player newPlayer = new Player();
            newPlayer.PlayerStatus = new PlayerStatus
            {
                Player = newPlayer,
                Status = PlayerStatusEnum.New
            };
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

        internal async Task<GenericResponse<bool>> StartSessionAsync(PlayerSessionRequest request)
        {
            _logger.LogInformation("Starting session", request);

            if (!await ValidateSessionStatusAsync(request.SessionId, SessionStatusEnum.New))
            {
                _logger.LogInformation("Session did not exist to start");
                return GenericResponse<bool>.Error("Session does not exist");
            }
            if (!await ValidateSessionMasterAsync(request.SessionId, request.PlayerId))
            {
                _logger.LogInformation("Request to start session from player other than the session master");
                return GenericResponse<bool>.Error("You do not have the authority to start the session");
            }
            _logger.LogInformation("Session cleared to start", request);

            await _playerStatusService.UpdateAllPlayersForSessionAsync(request.SessionId, PlayerStatusEnum.Waiting);            

            _logger.LogTrace("Sending update to clients");
            await _lobbyHub.SendStartingSessionAsync(request.SessionId);

            await Task.Delay(TimeSpan.FromSeconds(5));

            await _roundService.PrepareRoundsAsync(request.SessionId);

            return GenericResponse<bool>.Ok(true);
        }

        internal async Task<GenericResponse<JoinSessionResponse>> JoinSessionAsync(JoinSessionRequest request)
        {
            _logger.LogTrace($"Starting session join");
            var password = request.Password;

            var session = await GetSessionFromPasswordAsync(password);

            if (session == null)
            {
                _logger.LogInformation("Session doesn't exist");
                return NewResponse.Error<JoinSessionResponse>($"Session doesn't exist with password: {password}");
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
            newPlayer.PlayerStatus = new PlayerStatus
            {
                Player = newPlayer,
                Status = PlayerStatusEnum.New
            };

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
            return await _sessionRepository.SingleOrDefaultAsync(session => session.Password.ToLower() == password.ToLower());
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
