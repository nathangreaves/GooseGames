using Entities.Fuji;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Fuji
{
    public interface IPlayerRepository : ICommonRepository<Player>
    {
        Task DeleteUnreadyPlayersAsync(Guid sessionId);
        Task<int> GetNextPlayerNumberAsync(Guid sessionId);
        Task<List<Player>> GetForSessionIncludePlayedCardsAsync(Guid sessionId);
    }
}
