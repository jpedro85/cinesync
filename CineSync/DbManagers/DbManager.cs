using CineSync.Data;
using CineSync.Core.Repository;
using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CineSync.DbManagers
{
    public class DbManager<TEntity> where TEntity : class
    {
        protected readonly ILoggerStrategy _logger;
        protected readonly IUnitOfWorkAsync _unitOfWork;
        protected readonly IRepositoryAsync<TEntity> _repository;

        public DbManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger)
        {
            _unitOfWork = unitOfWork;
            _repository =  _unitOfWork.GetRepositoryAsync<TEntity>();
            _logger = logger;
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            _logger.Log($"Attemtping to save the entity {entity.GetType().Name} to the database", LogTypes.DEBUG);
            await _repository.InsertAsync(entity);
            bool result = await _unitOfWork.SaveChangesAsync();
            _logger.Log($"Saved entity {entity.GetType().Name} successfully to the database", LogTypes.DEBUG);
            return result;
        }

        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            _logger.Log($"Attemtping to remove the entity {entity.GetType()} to the database", LogTypes.DEBUG);
            _repository.Delete(entity);
            bool result = await _unitOfWork.SaveChangesAsync();
            _logger.Log($"Attemtping to remove the entity {entity.ToString()} to the database", LogTypes.DEBUG);
            return result;
        }

        public virtual async Task<TEntity?> GetByIdAsync(uint id)
        {
            return await _repository.GetAsync(id);
        }

        // INFO: Check the actual value of this function after implmentation of by conditions
        // public virtual async Task<TEntity?> GetByValuesAsync(params object[] objects)
        // {
        //     return await _dbContext.Set<TEntity>().FindAsync(objects);
        // }

        public virtual async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            _logger.Log("Fetching entities based on condition.", LogTypes.DEBUG);
            IEnumerable<TEntity> entities = await _repository.GetByConditionAsync(predicate, includes);
            _logger.Log("Fetched entities successfully.", LogTypes.DEBUG);
            return entities;
        }

        public virtual async Task<TEntity?> GetFirstByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            _logger.Log("Fetching first entity based on condition.", LogTypes.DEBUG);
            TEntity? entity = await _repository.GetFirstByConditionAsync(predicate, includes);
            _logger.Log("Fetched first entity successfully.", LogTypes.DEBUG);
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
