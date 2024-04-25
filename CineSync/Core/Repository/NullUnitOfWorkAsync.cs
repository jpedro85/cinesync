using CineSync.Data;

namespace CineSync.Core.Repository
{
    public sealed class NullUnitOfWorkAsync : IUnitOfWorkAsync
    {
        public static NullUnitOfWorkAsync Instance { get; } = new NullUnitOfWorkAsync();

        private NullUnitOfWorkAsync()
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

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class
        {
            return NullRepositoryAsync<TEntity>.Instance;
        }

        public Task<bool> SaveChangesAsync()
        {
            return Task.FromResult(false);
        }
    }
}
