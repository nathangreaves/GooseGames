using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Global
{
    public interface ISessionRepository : ICommonRepository<Entities.Global.Session>
    {
        Task AbandonSessionsOlderThanAsync(DateTime createdBeforeUtc);
        Task AbandonSessionsOlderThanAsync(string password, DateTime createdBeforeUtc);
    }
}
