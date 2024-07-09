using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineSync.Core.Repository
{
    public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        public Repository(IFactory factory, TContext context)
        {
            Factory = factory;
            Context = context;
        }

        protected IFactory Factory { get; set; }
        protected TContext Context { get; set; }

        public IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>();
        }

        public TEntity? Get(uint id)
        {
            return Context.Set<TEntity>()?.Find(id);
        }

        public IEnumerable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.Where(predicate).ToList();
        }

        public IEnumerable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query.Where(predicate).ToList();
        }

        public TEntity? GetFirstByCondition(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.FirstOrDefault(predicate);
        }

        public TEntity? Create(params object?[]? args)
        {
            return Factory.Create<TEntity>(args);
        }

		public void Attach(TEntity item)
		{
			Context.Entry(item).State = EntityState.Detached;
		}

		public void Dettach(TEntity item) 
        {
            Context.Entry(item).State = EntityState.Detached;
        }

        public void Insert(TEntity item)
        {
            Context.Set<TEntity>().Add(item);
        }

        public void Update(TEntity item)
        {
            Context.Entry(item).State = EntityState.Modified;
        }

        public void DeleteRange(IEnumerable<TEntity> items)
        {
            Context.RemoveRange(items);
        }

        public void Delete(TEntity item)
        {
            Context.Remove(item);
        }

        public void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty?>> expression) where TProperty : class
        {
            Context.Entry(entity).Reference(expression).Load();
        }

        public void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class
        {
            Context.Entry(entity).Collection(expression).Load();
        }

        public void Ensure<TProperty>(TEntity entity, Expression<Func<TEntity, ICollection<TProperty>>> expression) where TProperty : class
        {
            var parameter = expression.Parameters[0];
            var body = Expression.Convert(expression.Body, typeof(IEnumerable<TProperty>));
            Context.Entry(entity).Collection(
                Expression.Lambda<Func<TEntity, IEnumerable<TProperty>>>(body, parameter)).Load();
        }
    }
}
