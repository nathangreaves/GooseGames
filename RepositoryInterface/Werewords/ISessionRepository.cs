using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Entities.Werewords;

namespace RepositoryInterface.Werewords
{
    public interface ISessionRepository : ICommonRepository<Session>
    {
        Task AbandonSessionsOlderThanAsync(Guid excludeSessionId, DateTime createdBeforeUtc);
        Task AbandonSessionsOlderThanAsync(string password, DateTime createdBeforeUtc);
    }
}
