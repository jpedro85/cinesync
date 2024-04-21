using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    public class MovieManager(ApplicationDbContext dbContext) : DbManager<Movie>(dbContext)
    {
        public async Task<Movie> GetByTmdbId(int tmdbId)
        {
            var result = await GetByConditionAsync(movie => movie.MovieId == tmdbId);
            if (result.Count() != 0)
                return result.First();
            return null;
        }

        //public override async Task<bool> AddAsync(Movie movie)
        //{

        //    //foreach( Genre genre in movie.Genres )
        //    //{
        //    //    if (DbContext.Set<Genre>().Any(g => g.TmdbId == genre.TmdbId))
        //    //        continue;

        //    //    DbContext.Set<Genre>().Add(genre);
        //    //}

        //    DbContext.Set<Movie>().Add(movie);
        //    return await DbContext.SaveChangesAsync() > 0;
        //}
    }
}
