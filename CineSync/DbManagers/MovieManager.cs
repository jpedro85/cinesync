using CineSync.Data;
using CineSync.Data.Models;
using CineSync.Utils.Logger;

namespace CineSync.DbManagers
{
    public class MovieManager(ApplicationDbContext dbContext, ILoggerStrategy logger) : DbManager<Movie>(dbContext, logger)
    {
        public async Task<Movie?> GetByTmdbId(int tmdbId)
        {
            return await GetFirstByConditionAsync(movie => movie.MovieId == tmdbId, "Genres");
        }

    }
}
