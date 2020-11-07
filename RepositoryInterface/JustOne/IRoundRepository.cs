using Entities.JustOne;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.JustOne
{
    public interface IRoundRepository : ICommonRepository<Round>
    {
        Task<Round> GetCurrentRoundForGameAsync(Guid gameId);
        Task<int> RemoveRoundsForGameAsync(Guid gameId, int numberOfRoundsToRemove);
    }
}
