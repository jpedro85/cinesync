using CineSync.Core.Logger;
using CineSync.Data;
using CineSync.Core.Repository;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    public class MovieManager(IUnitOfWorkAsync unitOfWork,  ILoggerStrategy logger) : DbManager<Movie>(unitOfWork, logger)
    {
        public async Task<Movie?> GetByTmdbId(int tmdbId)
        {
            return await GetFirstByConditionAsync(movie => movie.MovieId == tmdbId, "Genres");
        }

    }
}
