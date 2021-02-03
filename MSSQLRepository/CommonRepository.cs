using Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using RepositoryInterface;

namespace MSSQLRepository
{
    public class CommonRepository<T> : ICommonRepository<T> where T : class, IHasGuidId, IHasCreatedUtc
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
            var queryable = DbContext.Set<T>()
                .AsQueryable();

            return await queryable.Where(func).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<T>> FilterAsync(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] includes)
        {
            var dbSet = DbContext.Set<T>();
            foreach (var include in includes)
            {
                dbSet.Include(include);
            }
            var queryable = dbSet
                .AsQueryable();

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

        public async Task<Dictionary<Guid, TProperty>> GetPropertyDictionaryAsync<TProperty>(IEnumerable<Guid> ids, Func<T, TProperty> select)
        {
            var idsList = ids as List<Guid> ?? ids.ToList();
            return await DbContext.Set<T>().Where(x => idsList.Contains(x.Id)).ToDictionaryAsync(k => k.Id, select).ConfigureAwait(false);
        }
        
        public async Task<Dictionary<Guid, TProperty>> GetPropertyForFilterAsync<TProperty>(Expression<Func<T, bool>> filter, Expression<Func<T, KeyValuePair<Guid, TProperty>>> select)
        {
            return await DbContext.Set<T>().Where(filter).Select(select).ToDictionaryAsync(k => k.Key, v => v.Value).ConfigureAwait(false);
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
            entity.CreatedUtc = DateTime.UtcNow;
            var lastUpdateEntity = entity as IHasLastUpdatedUtc;
            if (lastUpdateEntity != null)
            {
                lastUpdateEntity.LastUpdatedUtc = DateTime.UtcNow;
            }

            DbContext.Add(entity);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            foreach (var item in entities)
            {
                item.CreatedUtc = DateTime.UtcNow;
                var lastUpdateEntity = item as IHasLastUpdatedUtc;
                if (lastUpdateEntity != null)
                {
                    lastUpdateEntity.LastUpdatedUtc = DateTime.UtcNow;
                }
                DbContext.Add(item);
            }

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(T entity)
        {
            var lastUpdateEntity = entity as IHasLastUpdatedUtc;
            if (lastUpdateEntity != null)
            {
                lastUpdateEntity.LastUpdatedUtc = DateTime.UtcNow;
            }

            DbContext.Update(entity);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                var lastUpdateEntity = entity as IHasLastUpdatedUtc;
                if (lastUpdateEntity != null)
                {
                    lastUpdateEntity.LastUpdatedUtc = DateTime.UtcNow;
                }

                DbContext.Update(entity);
            }

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(T entity)   
        {
            DbContext.Remove(entity);

            await DbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual void Detach(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Detached;
        }
    }
}
