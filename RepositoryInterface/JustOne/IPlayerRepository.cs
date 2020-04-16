using Entities.JustOne;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.JustOne
{
    public interface IPlayerRepository : ICommonRepository<Player>
    {
        Task<string> GetActivePlayerConnectionIdAsync(Guid roundId);
        Task DeleteUnreadyPlayersAsync(Guid sessionId);
        Task<int> GetNextPlayerNumberAsync(Guid sessionId);
    }
}
