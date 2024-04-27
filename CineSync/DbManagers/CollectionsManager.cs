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

        public CollectionsManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
        }

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

        private async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userRepository.GetFirstByConditionAsync(user => user.Id == userId) ?? throw new Exception("User not found");
        }

        private bool IsMovieInCollection(Movie movie, MovieCollection collection)
        {
            return collection.CollectionMovies!.Any(cm => cm.MovieId == movie.Id);
        }
    }
}
