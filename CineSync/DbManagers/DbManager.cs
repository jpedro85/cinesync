using CineSync.Data;
using CineSync.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineSync.DbManagers
{
	public class DbManager<TEntity> where TEntity : class
	{
		protected readonly ApplicationDbContext DbContext;

		public DbManager(ApplicationDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<bool> AddAsync(TEntity entity)
		{
			DbContext.Set<TEntity>().Add(entity);
			return await DbContext.SaveChangesAsync() > 0;
		}

		public async Task<bool> RemoveAsync(TEntity entity)
		{
			DbContext.Set<TEntity>().Remove(entity);
			return await DbContext.SaveChangesAsync() > 0;
		}

		public async Task<TEntity?> GetByIdAsync(uint movieId)
		{
			return await DbContext.Set<TEntity>().FindAsync(movieId);
		}

		public async Task<TEntity?> GetByValuesAsync(params object[] objects)
		{
			return await DbContext.Set<TEntity>().FindAsync( objects );
		}

		public async Task< List<TEntity> > GetAllAsync()
		{
			return await DbContext.Set<TEntity>().ToListAsync();
		}
	}
}
