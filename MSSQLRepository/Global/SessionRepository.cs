using Entities.Global.Enums;
using Entities.Global;
using MSSQLRepository.Contexts;
using RepositoryInterface.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MSSQLRepository.Global
{
    public class SessionRepository : CommonRepository<Session>, ISessionRepository
    {
        protected GlobalContext Context => (GlobalContext)DbContext;

        public SessionRepository(GlobalContext context) : base(context)
        {

        }

        public async Task AbandonSessionsOlderThanAsync(string password, DateTime cutOffDateTimeUtc)
        {
            var sessionsToRemove = await Context.Sessions.Where(x => x.Password.ToLower() == password.ToLower()
               && (x.Status == SessionStatusEnum.InProgress || x.Status == SessionStatusEnum.Lobby)
               && x.LastUpdatedUtc < cutOffDateTimeUtc).ToListAsync().ConfigureAwait(false);

            if (sessionsToRemove.Any())
            {
                foreach (var sessionToRemove in sessionsToRemove)
                {
                    sessionToRemove.Status = SessionStatusEnum.Abandoned;
                    sessionToRemove.Password = Guid.NewGuid().ToString();
                    await UpdateAsync(sessionToRemove).ConfigureAwait(false);
                }
            }

        }

        public async Task AbandonSessionsOlderThanAsync(DateTime cutOffDateTimeUtc)
        {
            var sessionsToRemove = await Context.Sessions.Where(x =>
            (x.Status == SessionStatusEnum.InProgress || x.Status == SessionStatusEnum.Lobby)
            && x.LastUpdatedUtc < cutOffDateTimeUtc).ToListAsync().ConfigureAwait(false);

            if (sessionsToRemove.Any())
            {
                foreach (var sessionToRemove in sessionsToRemove)
                {
                    sessionToRemove.Status = SessionStatusEnum.Abandoned;
                    sessionToRemove.Password = Guid.NewGuid().ToString();
                    await UpdateAsync(sessionToRemove).ConfigureAwait(false);
                }
            }
        }
    }
}
