using CineSync.Core.Repository;
using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using System.Linq.Expressions;
using CineSync.Data.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Provides common database operations for any entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity this manager handles.</typeparam>
    public class DbManager<TEntity> where TEntity : class
    {
        protected readonly ILoggerStrategy _logger;
        protected readonly IUnitOfWorkAsync _unitOfWork;
        protected readonly IRepositoryAsync<TEntity> _repository;
        protected readonly Type _entityType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbManager{TEntity}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for managing transactions.</param>
        /// <param name="logger">The logger for logging activity within this manager.</param>
        public DbManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger)
        {
            _entityType = typeof(TEntity);
            _unitOfWork = unitOfWork;
            _repository =  _unitOfWork.GetRepositoryAsync<TEntity>();
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously adds an entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task resulting in a boolean indicating whether the add was successful.</returns>
        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            _logger.Log($"Attemtping to save the entity {entity.GetType().Name} to the database", LogTypes.DEBUG);
            await _repository.InsertAsync(entity);
            bool result = await _unitOfWork.SaveChangesAsync();
            _logger.Log($"Saved entity {entity.GetType().Name} successfully to the database", LogTypes.DEBUG);
            return result;
        }

        /// <summary>
        /// Asynchronously removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>A task resulting in a boolean indicating whether the removal was successful.</returns>
        public virtual async Task<bool> RemoveAsync(TEntity entity)
        {
            _logger.Log($"Attempting to remove the entity {entity.GetType()} to the database", LogTypes.DEBUG);
            _repository.Delete(entity);
            bool result = await _unitOfWork.SaveChangesAsync();
            _logger.Log($"Attempting to remove the entity {entity.ToString()} to the database", LogTypes.DEBUG);
            return result;
        }

        /// <summary>
        /// Asynchronously retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>A task resulting in the entity if found; otherwise, null.</returns>
        public virtual async Task<TEntity?> GetByIdAsync(uint id)
        {
            return await _repository.GetAsync(id);
        }

        /// <summary>
        /// Asynchronously retrieves entities that meet a specific condition.
        /// </summary>
        /// <param name="predicate">A lambda expression representing the condition to filter the entities.</param>
        /// <param name="includes">Optional parameter for eager loading of related entities.</param>
        /// <returns>A task resulting in a collection of entities meeting the condition.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            _logger.Log("Fetching entities based on condition.", LogTypes.DEBUG);
            IEnumerable<TEntity> entities = await _repository.GetByConditionAsync(predicate, includes);
            _logger.Log("Fetched entities successfully.", LogTypes.DEBUG);
            return entities;
        }

        /// <summary>
        /// Asynchronously retrieves the first entity that meets a specific condition.
        /// </summary>
        /// <param name="predicate">A lambda expression representing the condition to filter the entity.</param>
        /// <param name="includes">Optional parameter for eager loading of related entities.</param>
        /// <returns>A task resulting in the first entity meeting the condition if any; otherwise, null.</returns>
        public virtual async Task<TEntity?> GetFirstByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            _logger.Log("Fetching first entity based on condition.", LogTypes.DEBUG);
            TEntity? entity = await _repository.GetFirstByConditionAsync(predicate, includes);
            _logger.Log("Fetched first entity successfully.", LogTypes.DEBUG);
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves all entities of type TEntity.
        /// </summary>
        /// <returns>A task resulting in a collection of all entities of type TEntity.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }


        /// <summary>
        /// Upadates all specifyed TEntity properties.
        /// </summary>
        /// <param name="entity">The Entity to update.</param>
        /// <param name="properties"> The properties to update.</param>
        /// <returns>Returns True if upddated false otherwise.</returns>
        public async Task<bool> UpdateEntity( TEntity entity, params string[] properties)
        {
            TEntity? _dbentity = await _repository.GetFirstByConditionAsync(m => m.Equals(entity));

            if (_dbentity == null) return false;

            PropertyInfo[] messageProperrties = _entityType.GetProperties();
            PropertyInfo? objectProperty;

            foreach (var property in properties)
            {
                objectProperty = messageProperrties.Where(p => p.Name == property).FirstOrDefault();

                if (objectProperty == null)
                    throw new ArgumentException($"'{property}' is not a valid property or field of type {_entityType.Name}.");

                objectProperty.SetValue(_dbentity, objectProperty.GetValue(entity));
            }

            return await _unitOfWork.SaveChangesAsync();
        }

        public void Dettach( TEntity entity)
        {
            _repository.Dettach(entity);
        }

		public void Attach(TEntity entity)
		{
			_repository.Attach(entity);
		}
	}
}
