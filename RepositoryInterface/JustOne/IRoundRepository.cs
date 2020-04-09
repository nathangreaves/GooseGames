using Entities.JustOne;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.JustOne
{
    public interface IRoundRepository : ICommonRepository<Round>
    {
        Task<Round> GetCurrentRoundForSessionAsync(Guid sessionId);
        Task RemoveRoundsAsync(Guid sessionId, int numberOfRoundsToRemove);
    }
}
