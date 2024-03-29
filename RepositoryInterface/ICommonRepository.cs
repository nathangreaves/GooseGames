﻿using Entities.Common;
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
        Task<List<T>> FilterAsync(Expression<Func<T, bool>> func);
        Task<List<T>> FilterAsync(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] includes);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> func);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> func);
        Task<TProperty> GetPropertyAsync<TProperty>(Guid id, Expression<Func<T, TProperty>> select);
        Task<Dictionary<Guid, TProperty>> GetPropertyForFilterAsync<TProperty>(Expression<Func<T, bool>> filter, Expression<Func<T, KeyValuePair<Guid, TProperty>>> select);
        Task<Dictionary<Guid, TProperty>> GetPropertyDictionaryAsync<TProperty>(IEnumerable<Guid> ids, Func<T, TProperty> select);
        Task<bool> SingleResultMatchesAsync(Guid id, Expression<Func<T, bool>> func);
        Task<int> CountAsync(Expression<Func<T, bool>> func);

        void Detach(T entity);
    }
}
