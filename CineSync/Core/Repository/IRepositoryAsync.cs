using CineSync.Data;

namespace CineSync.Core.Repository
{
    public interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : Item
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task InsertAsync(TEntity item);
    }
}
