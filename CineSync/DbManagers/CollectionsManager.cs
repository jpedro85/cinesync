using CineSync.Core.Logger;
using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    public class CollectionsManager(ApplicationDbContext dbContext, ILoggerStrategy logger) : DbManager<MovieCollection>(dbContext, logger)
    {
        public async Task AddMovieAsync(MovieCollection collection, Movie movie)
        {

            CollectionsMovies newMovie = new CollectionsMovies();
            newMovie.MovieCollection = collection;
            newMovie.Movie = movie;
            newMovie.MovieId = movie.Id;
            newMovie.MovieCollectionId = collection.Id;

            if (collection.CollectionMovies == null)
                collection.CollectionMovies = new List<CollectionsMovies>();

            if (collection.CollectionMovies.Contains(newMovie))
            {
                collection.CollectionMovies.Add(newMovie);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveMovieAsync(MovieCollection collection, Movie movie)
        {
            if (collection.CollectionMovies != null)
            {
                foreach (var movieCollection in collection.CollectionMovies)
                {
                    if (movieCollection.MovieId == movie.Id)
                    {
                        collection.CollectionMovies.Remove(movieCollection);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

    }
}
