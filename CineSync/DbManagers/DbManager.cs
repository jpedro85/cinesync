using CineSync.Data;
using CineSync.Utils.Logger;
using CineSync.Utils.Logger.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineSync.DbManagers
{
    public class DbManager<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly ILoggerStrategy _logger;

        public DbManager(ApplicationDbContext dbContext, ILoggerStrategy logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            _logger.Log($"Attemtping to save the entity {entity.GetType()} to the database",LogTypes.DEBUG);
            _dbContext.Set<TEntity>().Add(entity);
            bool result = await _dbContext.SaveChangesAsync() > 0;
            _logger.Log($"Saved entity {entity.GetType()} successfully to the database",LogTypes.DEBUG);
            return result;
        }

        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            _logger.Log($"Attemtping to remove the entity {entity.ToString()} to the database",LogTypes.DEBUG);
            _dbContext.Set<TEntity>().Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public virtual async Task<TEntity?> GetByIdAsync(uint id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<TEntity?> GetByValuesAsync(params object[] objects)
        {
            return await _dbContext.Set<TEntity>().FindAsync(objects);
        }

        public virtual async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(predicate).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }
    }
}
