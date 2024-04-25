using Microsoft.EntityFrameworkCore;

namespace CineSync.Core.Repository
{
    public class UnitOfWorkAsync<TContext> : UnitOfWork<TContext>, IUnitOfWorkAsync where TContext : DbContext
    {
        public UnitOfWorkAsync( TContext context, IFactory factory )
            : base( context, factory )
        {
        }

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity>() as IRepositoryAsync<TEntity> ??
                   NullRepositoryAsync<TEntity>.Instance;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync() > 0;
        }

        protected override IRepository<TEntity> CreateRepository<TEntity>()
        {
            return new RepositoryAsync<TEntity,TContext>( Factory, Context );
        }

    }
}
