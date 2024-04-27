using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Core.Logger.Enums;

namespace CineSync.DbManagers
{
    public class MovieCollectionsManager : DbManager<MovieCollection>
    {
        private readonly IRepositoryAsync<ApplicationUser> _userRepository;
        private readonly IRepositoryAsync<CollectionsMovies> _collectionsRepository;

        public MovieCollectionsManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
            _collectionsRepository = _unitOfWork.GetRepositoryAsync<CollectionsMovies>();
        }

        public async Task<bool> InitializeUserCollectionsAsync(string userId)
        {
            var user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var collectionNames = new List<string> { "Favorites", "Watched", "Classified", "Watch Later" };

            foreach (var name in collectionNames)
            {
                var newCollection = new MovieCollection
                {
                    Name = name,
                    IsPublic = false
                };

                user.Collections.Add(newCollection);
            }

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> AddMovieToCollectionAsync(string userId, string collectionName, Movie movie)
        {
            var user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var collections = user!.Collections;

            var collection = collections!.FirstOrDefault(collection => collection.Name == collectionName);
            if (collection == null)
            {
                throw new Exception("Collection not found");
            }

            if (collection.CollectionMovies.Any(cm => cm.MovieId == movie.Id))
            {
                _logger.Log("Movie is already in the collection", LogTypes.WARN);
                return true;
            }

            var collectionsMovies = new CollectionsMovies
            {
                MovieId = movie.Id,
                MovieCollectionId = collection.Id,
            };

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> RemoveMovieFromCollectionAsync(string userId, string collectionName, Movie movie)
        {
            var user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var collections = user!.Collections;

            var collection = collections!.FirstOrDefault(collection => collection.Name == collectionName);
            if (collection == null)
            {
                throw new Exception("Collection not found");
            }

            var movieToRemove = collection.CollectionMovies.FirstOrDefault(cm => cm.MovieId == movie.MovieId);
            collection.CollectionMovies.Remove(movieToRemove);

            return await _unitOfWork.SaveChangesAsync();

        }

    }
}
