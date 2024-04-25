using CineSync.Data;

namespace CineSync.Core.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        void Begin();
        void Commit();
        void Rollback();

        void SaveChanges();
    }
}
