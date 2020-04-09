using Entities.Common;
using Entities.JustOne;
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
        public SessionRepository(JustOneContext dbContext) 
            : base(dbContext)
        {
        }
    }
}
