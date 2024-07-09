using CineSync.Data;
using System.Linq.Expressions;

namespace CineSync.Core.Repository
{
    public sealed class NullRepository<T> : IRepository<T> where T : class
    {
        public static NullRepository<T> Instance { get; } = new NullRepository<T>();

        private NullRepository()
        {
        }

        public IEnumerable<T> GetByCondition(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
        {
            return Enumerable.Empty<T>();
        }

        public T? GetFirstByCondition(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return null;
        }

        public T? Create( params object?[ ]? args )
        {
            return null;
        }

        public IQueryable<T> GetAll()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        public T? Get(uint id)
        {
            return null;
        }

        public IEnumerable<T> GetByCondition(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return  Enumerable.Empty<T>() ;
        }

        public void Attach(T item) 
        {
        }

        public void Dettach(T item) 
        {
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

        public void DeleteRange(IEnumerable<T> items) 
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
