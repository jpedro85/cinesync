using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages database operations for Movie entities, leveraging the generic DbManager functionalities for Movies.
    /// </summary>
    public class MovieManager : DbManager<Movie>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieManager"/> class, which manages the movie-related database operations.
        /// </summary>
        /// <param name="unitOfWork">The unit of work handling database transactions.</param>
        /// <param name="logger">The logger for recording operations and exceptions.</param>
        public MovieManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
        }

        /// <summary>
        /// Retrieves a movie by its TMDB ID including its associated genres.
        /// </summary>
        /// <param name="tmdbId">The TMDB ID of the movie to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the Movie entity if found, otherwise null.</returns>
        public async Task<Movie?> GetByTmdbId(int tmdbId)
        {
            // Attempts to find the first movie that matches the given TMDB ID, including its associated genres.
            return await GetFirstByConditionAsync(movie => movie.MovieId == tmdbId, "Genres");
        }

    }
}
