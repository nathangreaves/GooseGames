using Entities.JustOne;
using Models.Requests.JustOne;
using Models.Responses;
using Models.Responses.JustOne;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Services.JustOne
{
    public class SessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<GenericResponse<NewSessionResponse>> CreateSessionAsync(NewSessionRequest request)
        {
            var password = request.Password;

            if (await SessionExistsForPasswordAsync(password))
            {
                return NewResponse.Error<NewSessionResponse>($"Game already exists with password: {password}");
            }

            Session newSession = new Session
            {
                Password = password
            };

            await _sessionRepository.InsertAsync(newSession);

            return NewResponse.Ok(new NewSessionResponse 
            {
                GameId = newSession.Id
            });
        }

        internal async Task<GenericResponse<JoinSessionResponse>> JoinSessionAsync(JoinSessionRequest request)
        {
            var password = request.Password;

            var session = await GetSessionFromPasswordAsync(password);

            if (session == null)
            {
                return NewResponse.Error<JoinSessionResponse>($"Game doesn't exist with password: {password}");
            }

            return NewResponse.Ok(new JoinSessionResponse 
            {
                SessionId = session.Id
            });
        }

        private async Task<bool> SessionExistsForPasswordAsync(string password)
        {
            var found = await GetSessionFromPasswordAsync(password);

            return found != null;
        }

        private async Task<Session> GetSessionFromPasswordAsync(string password)
        {
            return await _sessionRepository.SingleOrDefaultAsync(session => session.Password.ToLower() == password.ToLower());
        }
    }
}
