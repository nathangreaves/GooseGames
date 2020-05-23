using Entities.Werewords;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Werewords
{
    public interface IPlayerRepository : ICommonRepository<Player>
    {
        Task DeleteUnreadyPlayersAsync(Guid sessionId);
        Task<int> GetNextPlayerNumberAsync(Guid sessionId);
    }
}
