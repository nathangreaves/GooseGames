using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities.Common;
using Entities.JustOne;

namespace RepositoryInterface.JustOne
{
    public interface ISessionRepository : ICommonRepository<Session>
    {
    }
}
