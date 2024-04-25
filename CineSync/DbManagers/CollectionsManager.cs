using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    public class CollectionsManager( IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : DbManager<MovieCollection>(unitOfWork, logger)
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
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RemoveMovieAsync(MovieCollection collection, Movie movie)
        {
            if (collection.CollectionMovies != null)
            {
                // Find the movie to remove by MovieId
                var movieToRemove = collection.CollectionMovies.FirstOrDefault(mc => mc.MovieId == movie.Id);

                // If a matching movie is found, remove it
                if (movieToRemove != null)
                {
                    collection.CollectionMovies.Remove(movieToRemove);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
 
        }

    }
}
