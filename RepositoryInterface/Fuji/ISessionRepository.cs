using Entities.Fuji;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface.Fuji
{
    public interface ISessionRepository : ICommonRepository<Session>
    {
        Task AbandonSessionsOlderThanAsync(Guid sessionId, DateTime dateTime);
    }
}
