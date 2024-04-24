using CineSync.Data;
using System.Linq.Expressions;

namespace CineSync.Core.Repository
{
    public sealed class NullRepository<T> : IRepository<T> where T : Item
    {
        public static NullRepository<T> Instance { get; } = new NullRepository<T>();

        private NullRepository()
        {
        }

        public T? Create( params object?[ ]? args )
        {
            return null;
        }

        public IQueryable<T> GetAll()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        public void Insert( T item )
        {
        }

        public void Update( T item )
        {
        }

        public void Delete( T item )
        {
        }

        public void Ensure<TProperty>( T entity, Expression<Func<T, TProperty?>> expression ) where TProperty : class
        {
        }

        public void Ensure<TProperty>( T entity, Expression<Func<T, IEnumerable<TProperty>>> expression ) where TProperty : class
        {
        }

        public void Ensure<TProperty>( T entity, Expression<Func<T, ICollection<TProperty>>> expression ) where TProperty : class
        {
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult( Enumerable.Empty<T>() );
        }

        public Task InsertAsync( T item )
        {
            return Task.CompletedTask;
        }
    }
}
