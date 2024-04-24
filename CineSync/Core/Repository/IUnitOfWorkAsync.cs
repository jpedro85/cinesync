using CineSync.Data;

namespace CineSync.Core.Repository
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : Item;

        Task SaveChangesAsync();
    }
}
