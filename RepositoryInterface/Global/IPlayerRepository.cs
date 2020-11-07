using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Global
{
    public interface IPlayerRepository : ICommonRepository<Entities.Global.Player>
    {
        Task<int> GetNextPlayerNumberAsync(Guid sessionId);
        Task<IEnumerable<Guid>> DeleteUnreadyPlayersAsync(Guid sessionId);
    }
}
