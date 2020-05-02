using Entities.Fuji;
using Entities.Fuji.Enums;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.Fuji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Fuji
{
    public class SessionRepository : CommonRepository<Session>, ISessionRepository
    {
        public FujiContext Context
        {
            get
            {
                return (FujiContext)DbContext;
            }
        }

        public SessionRepository(FujiContext dbContext)
            : base(dbContext)
        {
        }

        public async Task AbandonSessionsOlderThanAsync(Guid excludeSessionId, DateTime createdBeforeUtc)
        {
            var sessionsToRemove = await Context.Sessions.Where(x => x.Id != excludeSessionId
            && (x.StatusId == SessionStatusEnum.InProgress || x.StatusId == SessionStatusEnum.New)
            && x.CreatedUtc < createdBeforeUtc).ToListAsync().ConfigureAwait(false);

            if (sessionsToRemove.Any())
            {
                foreach (var sessionToRemove in sessionsToRemove)
                {
                    sessionToRemove.StatusId = SessionStatusEnum.Abandoned;
                    await UpdateAsync(sessionToRemove).ConfigureAwait(false);
                }
            }
        }
    }
}
