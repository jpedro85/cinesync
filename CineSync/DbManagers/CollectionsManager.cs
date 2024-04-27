using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Core.Logger.Enums;

namespace CineSync.DbManagers
{
    public class CollectionsManager : DbManager<MovieCollection>
    {
        private readonly IRepositoryAsync<ApplicationUser> _userRepository;

        /// <summary>
        /// Initializes a new instance of the CollectionsManager class.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for database operations.</param>
        /// <param name="logger">Logger for logging information and errors.</param>
        public CollectionsManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
        }

        /// <summary>
        /// Initializes default collections for a new user.
        /// </summary>
        /// <param name="userId">The user's identifier to whom collections will be added.</param>
        /// <returns>Returns true if collections are successfully added, otherwise false.</returns>
        public async Task<bool> InitializeUserCollectionsAsync(string userId)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            List<string> collectionNames = new List<string> { "Favorites", "Watched", "Classified", "Watch Later" };
            foreach (string name in collectionNames)
            {
                user.Collections!.Add(new MovieCollection { Name = name, IsPublic = false });
            }
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a movie to a specified collection for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="collectionName">The name of the collection to add the movie to.</param>
        /// <param name="movie">The movie to add to the collection.</param>
        /// <returns>Returns true if the movie is successfully added, otherwise false.</returns>
        /// <exception cref="Exception">Throws if the collection is not found.</exception>
        public async Task<bool> AddMovieToCollectionAsync(string userId, string collectionName, Movie movie)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            MovieCollection collection = user.Collections.FirstOrDefault(c => c.Name == collectionName) ?? throw new Exception("Collection not found");

            if (!IsMovieInCollection(movie, collection))
            {
                collection.CollectionMovies!.Add(new CollectionsMovies { MovieId = movie.Id, MovieCollectionId = collection.Id });
                return await _unitOfWork.SaveChangesAsync();
            }

            _logger.Log("Movie is already in the collection", LogTypes.WARN);
            return true;
        }

        /// <summary>
        /// Removes a movie from a specified collection for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="collectionName">The name of the collection to remove the movie from.</param>
        /// <param name="movie">The movie to remove from the collection.</param>
        /// <returns>Returns true if the movie is successfully removed, otherwise false.</returns>
        /// <exception cref="Exception">Throws if the collection is not found or the movie is not in the collection.</exception>
        public async Task<bool> RemoveMovieFromCollectionAsync(string userId, string collectionName, Movie movie)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            MovieCollection collection = user.Collections.FirstOrDefault(c => c.Name == collectionName) ?? throw new Exception("Collection not found");

            CollectionsMovies? movieToRemove = collection.CollectionMovies!.FirstOrDefault(cm => cm.MovieId == movie.MovieId);
            if (movieToRemove != null)
            {
                collection.CollectionMovies!.Remove(movieToRemove);
                return await _unitOfWork.SaveChangesAsync();
            }

            return false;
        }

        /// <summary>
        /// Retrieves a user by their identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user to retrieve.</param>
        /// <returns>The ApplicationUser corresponding to the specified userId.</returns>
        /// <exception cref="Exception">Throws if the user is not found.</exception>
        private async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetFirstByConditionAsync(user => user.Id == userId, "MovieCollection") ?? throw new Exception("User not found");
        }

        /// <summary>
        /// Checks if a movie is already included in a specified collection.
        /// </summary>
        /// <param name="movie">The movie to check.</param>
        /// <param name="collection">The collection to check against.</param>
        /// <returns>Returns true if the movie is already in the collection, otherwise false.</returns>
        private bool IsMovieInCollection(Movie movie, MovieCollection collection)
        {
            return collection.CollectionMovies!.Any(cm => cm.MovieId == movie.Id);
        }
    }
}
