using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities.JustOne.Filters
{
    public class SessionPasswordFilter : IEntityFilter<Session>
    {
        public SessionPasswordFilter(string password)
        {
            Password = password;
        }

        public string Password { get; }

        public IQueryable<Session> Filter(IQueryable<Session> queryable)
        {
            return queryable.Where(session => session.Password == Password);
        }
    }
}
