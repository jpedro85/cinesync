using System.Linq.Expressions;

namespace CineSync.Core.Repository
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> GetAll();

        TEntity? Create(params object?[]? args);

        void Insert(TEntity item);
        void Update(TEntity item);
        void Delete(TEntity item);

        void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> expression) where TProperty : class;
        void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class;
        void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, ICollection<TProperty>>> expression) where TProperty : class;
    }
}
