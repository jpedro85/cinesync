using CineSync.Data;
using System.Linq.Expressions;

namespace CineSync.Core.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        TEntity? Get(uint id);
        IEnumerable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        IEnumerable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);
        TEntity? GetFirstByCondition(Expression<Func<TEntity, bool>> predicate, params string[] includes);

        TEntity? Create(params object?[]? args);

        void Insert(TEntity item);
        void Update(TEntity item);
        void Delete(TEntity item);
        void DeleteRange(IEnumerable<TEntity> items);

        void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> expression) where TProperty : class;
        void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class;
        void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, ICollection<TProperty>>> expression) where TProperty : class;
    }
}
