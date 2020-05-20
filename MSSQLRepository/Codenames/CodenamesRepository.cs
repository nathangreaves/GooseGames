using Entities.Codenames;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Codenames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Codenames
{
    public class CodenamesRepository : ICodenamesRepository
    {
        private readonly CodenamesContext _context;

        public CodenamesRepository(CodenamesContext context)
        {
            _context = context;
        }


        public async Task<Session> GetSessionAsync(Guid id)
        {
            return await _context.Sessions.FindAsync(id).ConfigureAwait(false);
        }

        public async Task<CodenamesWord> GetWordAsync(Guid wordId)
        {
            return await _context.Words.FindAsync(wordId).ConfigureAwait(false);
        }

        public async Task<Session> GetSessionAndWordsAsync(string gameId)
        {
            var session = await _context.Sessions.Where(x => x.Password.ToLower() == gameId.ToLower())
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return await GetWordsForSessionAsync(session);
        }

        public async Task<Session> GetSessionAndWordsAsync(Guid id)
        {
            var session = await GetSessionAsync(id);
            return await GetWordsForSessionAsync(session);
        }

        private async Task<Session> GetWordsForSessionAsync(Session session)
        {
            if (session != null)
            {
                var words = await _context.Words.Where(x => x.SessionId == session.Id
                && x.SessionWordsId == session.SessionWordsId)
                    .ToListAsync().ConfigureAwait(false);

                session.Words = words;

                _context.Entry(session).State = EntityState.Unchanged;
                return session;
            }

            return null;
        }

        public async Task InsertSessionAsync(Session session)
        {
            session.CreatedUtc = DateTime.UtcNow;
            _context.Sessions.Add(session);

            await _context.SaveChangesAsync();
        }

        public async Task InsertWordsAsync(List<CodenamesWord> wordList)
        {
            foreach (var item in wordList)
            {
                item.CreatedUtc = DateTime.UtcNow;
                _context.Add(item);
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateSessionAsync(Session session)
        {
            _context.Update(session);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateWordAsync(CodenamesWord word)
        {
            _context.Update(word);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
