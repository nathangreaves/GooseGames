using Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using RepositoryInterface;

namespace PostGreRepository
{
    public class CommonRepository<T> : ICommonRepository<T> where T : class, IHasGuidId
    {
        public DbContext DbContext { get; }

        public CommonRepository(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<List<T>> FilterAsync(params IEntityFilter<T>[] filters)
        {
            var queryable = DbContext.Set<T>().AsQueryable();            
            foreach (var filter in filters)
            {
                queryable = filter.Filter(queryable);
            }

            return await queryable.ToListAsync().ConfigureAwait(false);
        }
        public async Task<List<T>> FilterAsync(Expression<Func<T, bool>> func)
        {
            var queryable = DbContext.Set<T>().AsQueryable();

            return await queryable.Where(func).ToListAsync().ConfigureAwait(false);
        }
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> func)
        {
            var queryable = DbContext.Set<T>().AsQueryable();

            return await queryable.Where(func).FirstOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> func)
        {
            var queryable = DbContext.Set<T>().AsQueryable();

            return await queryable.Where(func).SingleOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<TProperty> GetPropertyAsync<TProperty>(Guid id, Expression<Func<T, TProperty>> select)
        {
            return await DbContext.Set<T>().Where(x => x.Id == id).Select(select).SingleOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<bool> SingleResultMatchesAsync(Guid id, Expression<Func<T, bool>> func)
        {
            return (await DbContext.Set<T>().Where(x => x.Id == id).Where(func).CountAsync().ConfigureAwait(false)) == 1;
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> func)
        {
            return await DbContext.Set<T>().Where(func).CountAsync().ConfigureAwait(false);
        }

        public virtual async Task<T> GetAsync(Guid id)
        {
            return await DbContext.Set<T>().FindAsync(id).ConfigureAwait(false);
        }

        public async Task InsertAsync(T entity)
        {
            DbContext.Add(entity);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            DbContext.AddRange(entities);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity)
        {
            DbContext.Update(entity);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            DbContext.UpdateRange(entities);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(T entity)   
        {
            DbContext.Remove(entity);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
