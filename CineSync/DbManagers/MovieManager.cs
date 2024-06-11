using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages database operations for Movie entities, leveraging the generic DbManager functionalities for Movies.
    /// </summary>
    public class MovieManager : DbManager<Movie>
    {

        private readonly IRepositoryAsync<ApplicationUser> _userRepository;
        private readonly IRepositoryAsync<MovieCollection> _collectionsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieManager"/> class, which manages the movie-related database operations.
        /// </summary>
        /// <param name="unitOfWork">The unit of work handling database transactions.</param>
        /// <param name="logger">The logger for recording operations and exceptions.</param>
        public MovieManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
            _collectionsRepository = _unitOfWork.GetRepositoryAsync<MovieCollection>();
        }

        /// <summary>
        /// Retrieves a movie by its TMDB ID including its associated genres.
        /// </summary>
        /// <param name="tmdbId">The TMDB ID of the movie to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the Movie entity if found, otherwise null.</returns>
        public async Task<Movie?> GetByTmdbId(int tmdbId)
        {
            return await GetFirstByConditionAsync(movie => movie.MovieId == tmdbId, "Genres");
        }

        public async Task AddRating(int rating, int movieId, string userId)
        {
            Movie movie = await GetByTmdbId(movieId);

            if (movie == null)
            {
                _logger.Log("Invalid MovieId or Something went wrong while searching for Movie", LogTypes.WARN);
            }

            movie!.RatingCS = RecalculateRating(movie!.RatingCS, movie.VoteCount, rating);
            movie.VoteCount++;

            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId, "Collections");
            if (user == null || user.Collections == null || !user.Collections.Any())
            {
                _logger.Log("User not found or has no collections", LogTypes.WARN);
                return;
            }

            MovieCollection collection = user.Collections.FirstOrDefault(collection => collection.Name == "Classified")!;
            if (collection == null)
            {
                _logger.Log("Classified collection not found", LogTypes.WARN);
                return;
            }

            if (collection.CollectionMovies == null)
                collection.CollectionMovies = new List<CollectionsMovies>();

            if (!collection.CollectionMovies.Any(cm => cm.MovieId == movie.Id))
            {
                CollectionsMovies ratedMovie = new CollectionsMovies()
                {
                    MovieId = movie.Id,
                    MovieCollectionId = collection.Id
                };

                collection.CollectionMovies.Add(ratedMovie);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private float RecalculateRating(float currentRating, uint voteCount, int userRating)
        {
            float total = (currentRating * voteCount);
            _logger.Log(voteCount.ToString(), LogTypes.DEBUG);
            return (total + userRating) / (voteCount + 1);
        }

    }
}
