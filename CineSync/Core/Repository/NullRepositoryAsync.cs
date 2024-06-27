using CineSync.Data;
using System.Linq.Expressions;

namespace CineSync.Core.Repository
{
    public sealed class NullRepositoryAsync<T> : IRepositoryAsync<T> where T : class 
    {
        public static NullRepositoryAsync<T> Instance { get; } = new NullRepositoryAsync<T>();

        private NullRepositoryAsync() { }

        public T? Create( params object?[]? args )
        {
            return null;
        }

        public T?  Get( uint id )
        {
            return null;
        }

        public Task<T?>  GetAsync( uint id )
        {
            return Task.FromResult<T?>( null );
        }

        public IQueryable<T> GetAll()
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult( Enumerable.Empty<T>() );
        }

        public void Insert( T item )
        {
        }

        public  IEnumerable<T> GetByCondition(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return Enumerable.Empty<T>();
        }

        public IEnumerable<T> GetByCondition(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
        {
                return Enumerable.Empty<T>();
        }

        public IEnumerable<T> GetByCondition(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return Enumerable.Empty<T>();
        }

        public  Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return Task.FromResult( Enumerable.Empty<T>() );
        }

        public Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
        {
            return Task.FromResult( Enumerable.Empty<T>() );
        }

        public  T? GetFirstByCondition(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return null;
        }

        public  Task<T?> GetFirstByConditionAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return Task.FromResult<T?>( null );
        }

        public Task InsertAsync( T item )
        {
            return Task.FromResult(0);
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
    }
}
