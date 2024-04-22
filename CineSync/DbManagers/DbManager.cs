using CineSync.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineSync.DbManagers
{
    public class DbManager<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext DbContext;

        public DbManager(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<TEntity?> GetByIdAsync(uint id)
        {
            return await DbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity?> GetByValuesAsync(params object[] objects)
        {
            return await DbContext.Set<TEntity>().FindAsync(objects);
        }

        public virtual async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(predicate).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await DbContext.Set<TEntity>().ToListAsync();
        }
    }
}
