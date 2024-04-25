using CineSync.Data;

namespace CineSync.Core.Repository
{
    public sealed class NullUnitOfWork : IUnitOfWork
    {
        public static NullUnitOfWork Instance { get; } = new NullUnitOfWork();

        private NullUnitOfWork()
        {
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return NullRepository<TEntity>.Instance;
        }

        public void Begin()
        {
        }

        public void Commit()
        {
        }

        public void Dispose()
        {
        }

        public void Rollback()
        {
        }

        public void SaveChanges()
        {
        }
    }
}
