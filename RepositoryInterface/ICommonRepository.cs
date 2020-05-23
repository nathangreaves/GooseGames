using Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface
{
    public interface ICommonRepository<T> where T : IHasGuidId
    {
        Task<T> GetAsync(Guid id);
        Task InsertAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entity);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task<List<T>> FilterAsync(params IEntityFilter<T>[] filters); 
        Task<List<T>> FilterAsync(Expression<Func<T, bool>> func);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> func);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> func);
        Task<TProperty> GetPropertyAsync<TProperty>(Guid id, Expression<Func<T, TProperty>> select);
        Task<bool> SingleResultMatchesAsync(Guid id, Expression<Func<T, bool>> func);
        Task<int> CountAsync(Expression<Func<T, bool>> func);

        void Detach(T entity);
    }
}
