﻿using Entities.Werewords;
using Entities.Werewords.Enums;
using Microsoft.EntityFrameworkCore;
using MSSQLRepository.Contexts;
using RepositoryInterface.Werewords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLRepository.Werewords
{
    public class SessionRepository : CommonRepository<Session>, ISessionRepository
    {
        private WerewordsContext Context => (WerewordsContext)DbContext;

        public SessionRepository(WerewordsContext context) : base(context)
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
