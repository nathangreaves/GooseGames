using Entities.Common;
using Entities.JustOne;
using Entities.JustOne.Enums;
using Microsoft.EntityFrameworkCore;
using RepositoryInterface.JustOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.JustOne
{
    public class SessionRepository : CommonRepository<Session>, ISessionRepository
    {
        public JustOneContext Context
        {
            get
            {
                return (JustOneContext)DbContext;
            }
        }

        public SessionRepository(JustOneContext dbContext) 
            : base(dbContext)
        {
        }

        public async Task AbandonSessionsOlderThanAsync(Guid excludeSessionId, DateTime createdBeforeUtc)
        {
            var sessionsToRemove = await Context.Sessions.Where(x => x.Id != excludeSessionId
            && (x.StatusId == SessionStatusEnum.InProgress || x.StatusId == SessionStatusEnum.New)
            && x.CreatedUtc < createdBeforeUtc).ToListAsync();

            if (sessionsToRemove.Any())
            {
                foreach (var sessionToRemove in sessionsToRemove)
                {
                    sessionToRemove.StatusId = SessionStatusEnum.Abandoned;
                    await UpdateAsync(sessionToRemove);
                }
            }
        }
    }
}
