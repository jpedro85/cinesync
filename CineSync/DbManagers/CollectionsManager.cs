using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Core.Logger.Enums;
using Microsoft.IdentityModel.Tokens;

namespace CineSync.DbManagers
{
    public class CollectionsManager : DbManager<MovieCollection>
    {
        private readonly IRepositoryAsync<ApplicationUser> _userRepository;
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly ICollection<string> _defaultCollectionNames = new string[] { "Favorites", "Watched", "Classified", "Watch Later" };
        private readonly string WatchTimeCollection = "Watched";

        /// <summary>
        /// Initializes a new instance of the CollectionsManager class.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for database operations.</param>
        /// <param name="logger">Logger for logging information and errors.</param>
        public CollectionsManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
            _movieRepository = _unitOfWork.GetRepositoryAsync<Movie>();
        }

        /// <summary>
        /// Retrieves the collections of movies for a specified user asynchronously.
        /// </summary>
        /// <param name="userId">The identifier of the user whose collections are to be retrieved.</param>
        /// <returns>The collections of movies associated with the specified user.</returns>
        public async Task<ICollection<MovieCollection>> GetUserCollections(string userId)
        {
            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "Collections.CollectionMovies.Movie");
            return user!.Collections!;
        }
        
        public async Task<IEnumerable<MovieCollection>> GetUserDefaultCollectionsWithoutMovies(string userId)
        {
            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId,"Collections");
            return user?.Collections!.Where(c=> _defaultCollectionNames.Contains(c.Name));
        }

        /// <summary>
        /// Initializes default collections for a new user.
        /// </summary>
        /// <param name="userId">The user's identifier to whom collections will be added.</param>
        /// <returns>Returns true if collections are successfully added, otherwise false.</returns>
        public async Task<bool> InitializeUserCollectionsAsync(string userId)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            foreach (string name in _defaultCollectionNames)
            {
                user.Collections!.Add(new MovieCollection { Name = name, IsPublic = false, CollectionMovies = new List<CollectionsMovies>(0) });
            }
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a new collection asynchronously for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="collectionName">The name of the new collection to create.</param>
        /// <returns>Returns True if the collection is successfully created, otherwise False.</returns>
        public async Task<bool> CreateNewCollectionAsync(string userId, string collectionName)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            if (_defaultCollectionNames.Contains(collectionName))
            {
                return true;
            }
            user.Collections!.Add(new MovieCollection { Name = collectionName, IsPublic = false, CollectionMovies = new List<CollectionsMovies>(0) });
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a movie to a specified collection for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="collectionName">The name of the collection to add the movie to.</param>
        /// <param name="movieID">The movie to add to the collection.</param>
        /// <returns>Returns true if the movie is successfully added, otherwise false.</returns>
        /// <exception cref="Exception">Throws if the collection is not found.</exception>
        public async Task<bool> AddMovieToCollectionAsync(string userId, string collectionName, uint movieID)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            Movie movie = await _movieRepository.GetFirstByConditionAsync(m => m.Id == movieID) ?? throw new Exception("Movie not found");
            MovieCollection collection = user.Collections!.FirstOrDefault(c => c.Name == collectionName) ?? throw new Exception("Collection not found");
            if (collection.CollectionMovies.IsNullOrEmpty())
            {
                collection.CollectionMovies = new List<CollectionsMovies>();
            }

            if (IsMovieInCollection(movieID, collection))
            {
                _logger.Log("Movie is already in the collection", LogTypes.WARN);
                return true;
            }

            user.WatchTime += movie.RunTime;
            collection.CollectionMovies!.Add(new CollectionsMovies { MovieId = movie.Id, MovieCollectionId = collection.Id });
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a movie from a specified collection for a user.
        /// </summary>
        /// <param name="collectionId">The Id of the collection to remove the movie from.</param>
        /// <param name="movieId">The movie Id to remove from the collection.</param>
        /// <returns>Returns true if the movie is successfully removed, otherwise false.</returns>
        public async Task<bool> RemoveMovieFromCollectionAsync(string userId, uint collectionId, uint movieId)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            MovieCollection? collection = await _repository.GetFirstByConditionAsync(c => c.Id == collectionId);
            if (user == null || collection == null)
            {
                return false;
            }

            CollectionsMovies? movieToRemove = collection.CollectionMovies!.FirstOrDefault(cm => cm.MovieId == movieId);
            if (movieToRemove == null)
            {
                return false;
            }

            if (collection.Name == WatchTimeCollection)
            {
                if (user.WatchTime - movieToRemove.Movie.RunTime < 0)
                {
                    user.WatchTime = 0;
                }
                else
                {
                    user.WatchTime -= movieToRemove.Movie.RunTime;
                }
            }

            collection.CollectionMovies!.Remove(movieToRemove);
            return await _unitOfWork.SaveChangesAsync();
        }

        /// Removes a movie from a specified collection for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="collectionName">The name of the collection to remove the movie from.</param>
        /// <param name="movie">The movie to remove from the collection.</param>
        /// <returns>Returns true if the movie is successfully removed, otherwise false.</returns>
        /// <exception cref="Exception">Throws if the collection is not found or the movie is not in the collection.</exception>
        public async Task<bool> RemoveMovieFromCollectionAsync(string userId, string collectionName, uint movieId)
        {
            ApplicationUser user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            MovieCollection collection = user.Collections!.FirstOrDefault(c => c.Name == collectionName) ?? throw new Exception("Collection not found");
            CollectionsMovies? movieToRemove = collection.CollectionMovies!.FirstOrDefault(cm => cm.MovieId == movieId);
            if (movieToRemove == null)
            {
                return false;
            }

            if (collection.Name == WatchTimeCollection)
            {
                if (user.WatchTime - movieToRemove.Movie.RunTime < 0)
                {
                    user.WatchTime = 0;
                }
                else
                {
                    user.WatchTime -= movieToRemove.Movie.RunTime;
                }
            }
            collection.CollectionMovies!.Remove(movieToRemove);
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a specified collection for a user.
        /// </summary>
        /// <param name="collectionId">The Id of the collection to remove.</param>
        /// <returns>Returns true if the collection is successfully removed, otherwise false.</returns>
        public async Task<bool> RemoveCollectionAsync(uint collectionId)
        {
            MovieCollection? collection = await _repository.GetFirstByConditionAsync(c => c.Id == collectionId);
            if (collection == null)
            {
                return false;
            }

            return await RemoveAsync(collection);
        }


        /// <summary>
        /// Changes the Name of a collection.
        /// </summary>
        /// <param name="collectionId">The Id of the collection to change the Name of.</param>
        /// <param name="newCollectionName">The newName of the Collection.</param>
        /// <returns>True if sucessefull otherwise False</returns>
        public async Task<bool> ChangeCollectionName(uint collectionId, string newCollectionName)
        {
            MovieCollection? collection = await _repository.GetFirstByConditionAsync(c => c.Id == collectionId);
            if (collection == null)
            {
                return false;
            }

            collection.Name = newCollectionName;
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Changes the public state of a collection.
        /// </summary>
        /// <param name="collectionId">The id of the collection to change the state of.</param>
        /// <param name="newState">The nes isPublic sate of the collection.</param>
        /// <returns>True if sucessefull, otherwise false.</returns>
        public async Task<bool> ChangePublicSate(uint collectionId, bool newState)
        {
            MovieCollection? collection = await _repository.GetFirstByConditionAsync(c => c.Id == collectionId);
            if (collection == null)
            {
                return false;
            }

            collection.IsPublic = newState;
            return await _unitOfWork.SaveChangesAsync();
        }


        /// <summary>
        /// Retrieves a user by their identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user to retrieve.</param>
        /// <returns>The ApplicationUser corresponding to the specified userId.</returns>
        /// <exception cref="Exception">Throws if the user is not found.</exception>
        private async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetFirstByConditionAsync(user => user.Id == userId, "Collections") ?? throw new Exception("User not found");
        }

        /// <summary>
        /// Checks if a movie is already included in a specified collection.
        /// </summary>
        /// <param name="movie">The movie to check.</param>
        /// <param name="collection">The collection to check against.</param>
        /// <returns>Returns true if the movie is already in the collection, otherwise false.</returns>
        private bool IsMovieInCollection(uint movieId, MovieCollection collection)
        {
            return collection.CollectionMovies?.Any(cm => cm.MovieId == movieId) ?? false;
        }

        public async Task<bool> IsMovieInCollection(uint movieId, uint collectionID)
        {
            MovieCollection collection = await _repository.GetFirstByConditionAsync(collection => collection.Id == collectionID, "CollectionMovies");
            return collection.CollectionMovies?.Any(cm => cm.MovieId == movieId) ?? false;
        }
    }
}
