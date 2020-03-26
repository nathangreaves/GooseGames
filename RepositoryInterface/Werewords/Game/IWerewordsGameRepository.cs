using Entities.Werewords.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Werewords.Game
{
    public interface IWerewordsGameRepository
    {
        Task<Guid> Create(string password);
        Task<bool> ExistsWithPassword(string password);        
        Task<WerewordsGame> AddPlayer(string password);
    }
}
