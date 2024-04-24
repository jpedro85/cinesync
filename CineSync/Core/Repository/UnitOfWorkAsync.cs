using CineSync.Data;

namespace CineSync.Core.Repository
{
    public class UnitOfWorkAsync : UnitOfWork, IUnitOfWorkAsync
    {
        public UnitOfWorkAsync(ApplicationDbContext context, IFactory factory)
            : base(context, factory)
        {
        }

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : Item
        {
            return GetRepository<TEntity>() as IRepositoryAsync<TEntity> ??
                   NullRepositoryAsync<TEntity>.Instance;
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        protected override IRepository<TEntity> CreateRepository<TEntity>()
        {
            return new RepositoryAsync<TEntity>(Factory, Context);
        }

    }
}
