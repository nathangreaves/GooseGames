using Entities.Codenames;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Codenames
{
    public interface ICodenamesRepository
    {
        Task<Session> GetSessionAndWordsAsync(string gameId);
        Task<Session> GetSessionAndWordsAsync(Guid id);
        Task<Session> GetSessionAsync(Guid id);
        Task InsertSessionAsync(Session session);
        Task InsertWordsAsync(List<CodenamesWord> wordList);
        Task<CodenamesWord> GetWordAsync(Guid wordId);
        Task UpdateWordAsync(CodenamesWord word);
        Task UpdateSessionAsync(Session session);
    }
}
