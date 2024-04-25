using CineSync.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace CineSync.Core.Repository
{
    public class UnitOfWork<TContext> : IUnitOfWork, IDisposable where TContext: DbContext
    {
        public UnitOfWork( TContext context, IFactory factory )
        {
            Context = context;
            Factory = factory;
        }

        public    IFactory  Factory { get; }
        protected TContext  Context { get; }

        private IDbContextTransaction? Transaction { get; set; }

        public void Begin()
        {
            Transaction = Context.Database.BeginTransaction();
        }

        public void Commit()
        {
            Transaction?.Commit();
            Transaction = null;
        }

        public void Rollback()
        {
            Transaction?.Rollback();
            Transaction = null;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        private IDictionary<Type, object> Repositories { get; } = new Dictionary<Type, object>();

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if ( Repositories.ContainsKey( typeof( TEntity ) ) )
                return Repositories[typeof( TEntity )] as IRepository<TEntity> ??
                       NullRepository<TEntity>.Instance;

            var repository = CreateRepository<TEntity>();
            if ( repository != null ) Repositories.Add( typeof( TEntity ), repository );

            return repository ??
                   NullRepository<TEntity>.Instance;
        }

        protected virtual IRepository<TEntity> CreateRepository<TEntity>() where TEntity :  class
        {
            return new Repository<TEntity,TContext>( Factory, Context );
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
