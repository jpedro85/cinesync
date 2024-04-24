using CineSync.Data;

namespace CineSync.Core.Repository
{
    public sealed class NullUnitOfWorkAsync : IUnitOfWorkAsync
    {
        public static NullUnitOfWorkAsync Instance { get; } = new NullUnitOfWorkAsync();

        private NullUnitOfWorkAsync()
        {
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Item
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

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : Item
        {
            return NullRepositoryAsync<TEntity>.Instance;
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
