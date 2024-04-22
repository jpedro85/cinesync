using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    public class MovieManager(ApplicationDbContext dbContext) : DbManager<Movie>(dbContext)
    {
        public async Task<Movie> GetByTmdbId(int tmdbId)
        {
            var result = await GetByConditionAsync(movie => movie.MovieId == tmdbId, "Genres");
            if (result.Count() != 0)
                return result.First();
            return null;
        }

    }
}
