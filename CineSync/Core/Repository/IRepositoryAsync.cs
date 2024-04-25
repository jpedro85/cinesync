using CineSync.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;

namespace CineSync.Core.Repository
{
    public interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(uint id);
        Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate,  CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetFirstByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        Task InsertAsync(TEntity item);
    }
}
