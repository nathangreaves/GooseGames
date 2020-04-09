using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities.Common
{
    public interface IEntityFilter<T>
    {
        IQueryable<T> Filter(IQueryable<T> queryable);
    }
}
