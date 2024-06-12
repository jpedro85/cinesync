using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages user-related data transactions to the database.
    /// </summary>
    /// <remarks>
    /// The UserManager class is designed to perform operations that affect the ApplicationUser
    /// entities within the database, using the repository pattern to interact with the data layer.
    /// Inherits from DbManager for common database operations.
    /// </remarks>
    public class UserManager : DbManager<ApplicationUser>
    {
        private readonly IRepositoryAsync<Comment> _commentRepository;

        /// <summary>
        /// Initializes a new instance of the UserManager class.
        /// </summary>
        /// <param name="unitOfWork">Provides an interface for a unit of work mechanism to handle transactions.</param>
        /// <param name="logger">Provides logging capabilities to trace the operations and errors.</param>
        public UserManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _commentRepository = _unitOfWork.GetRepositoryAsync<Comment>();
        }

        /// <summary>
        /// Attempts to change the username of a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier for the user whose username is to be changed.</param>
        /// <param name="username">The new username to be assigned to the user.</param>
        /// <returns>
        /// A task that represents the asynchronous operation, containing a boolean value.
        /// True if the username was successfully changed; otherwise, false if the user does not exist,
        /// the username is already taken, or the new username is null or empty.
        /// </returns>
        /// <remarks>
        /// This method first ensures the user exists and that the proposed new username is not null or already in use.
        /// If these conditions are met, it updates the username and commits the changes to the database.
        /// </remarks>
        public async Task<bool> ChangeUsernameAsync(string userId, string username)
        {
            ApplicationUser user = await GetFirstByConditionAsync(u => u.Id == userId);
            if (user == null || string.IsNullOrEmpty(username) || await GetFirstByConditionAsync(user => user.UserName == username) != null)
            {
                return false;
            }

            user.UserName = username;
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> Follow(string userId, string userToFollowId)
        {
            if (userId == userToFollowId)
                return false;

            ApplicationUser user = await GetFirstByConditionAsync(u => u.Id == userId, "Following");
            ApplicationUser userToFollow = await GetFirstByConditionAsync(u => u.Id == userToFollowId, "Followers");

            if (user == null || userToFollow == null)
                return false;

            if (user.Following == null)
                user.Following = new List<ApplicationUser>();

            if (userToFollow.Followers == null)
                userToFollow.Followers = new List<ApplicationUser>();

            if (!user.Following.Contains(userToFollow))
            {
                user.Following.Add(userToFollow);
                user.FollowingCount = (uint)user.Following.Count;

                userToFollow.Followers.Add(user);
                userToFollow.FollowersCount = (uint)userToFollow.Followers.Count;
            }
            else
                return false;

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UnFollow(string userId, string userToFollowId)
        {
            if (userId == userToFollowId)
                return false;

            ApplicationUser user = await GetFirstByConditionAsync(u => u.Id == userId, "Following");
            ApplicationUser userToFollow = await GetFirstByConditionAsync(u => u.Id == userToFollowId, "Followers");

            if (user == null || userToFollow == null)
                return false;

            if (user.Following != null)
            {
                user.Following.Remove(userToFollow);
                user.FollowingCount = (uint)user.Following.Count;
            }

            if (userToFollow.Followers != null)
            {
                userToFollow.Followers.Remove(user);
                userToFollow.FollowersCount = (uint)userToFollow.Followers.Count;
            }

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> ChangeProfilePictureAsync(string userId, byte[] image, string contentType)
        {
            ApplicationUser user = await GetFirstByConditionAsync(u => u.Id == userId, "UserImage");
            if (user == null || image == null || contentType == null)
            {
                return false;
            }

            if (user.UserImage != null)
            {
                _logger.Log("There is UserImage", LogTypes.DEBUG);
                _logger.Log("Updating the User " + userId + " profile Image", LogTypes.DEBUG);
                user.UserImage.ImageData = image;
                user.UserImage.ContentType = contentType;
                _logger.Log("The User " + user.Id + " profile Image was updated ", LogTypes.DEBUG);
            }
            else
            {
                _logger.Log("There is no UserImage", LogTypes.DEBUG);
                _logger.Log("Creating a new User Image", LogTypes.DEBUG);
                UserImage userImage = new UserImage
                {
                    UserId = user.Id,
                    ImageData = image,
                    ContentType = contentType
                };
                _logger.Log("Created a new User Image", LogTypes.DEBUG);
                user.UserImage = userImage;
            }

            _logger.Log("Saving User Image changes to the database", LogTypes.WARN);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteAccountAsync(string userId)
        {
            _logger.Log("Fetching the User and the ghostUser", LogTypes.DEBUG);
            var userTask = GetFirstByConditionAsync(u => u.Id == userId, "Collections", "Followers", "Following");
            var ghostUserTask = GetFirstByConditionAsync(u => u.Id == "0");
            await Task.WhenAll(userTask, ghostUserTask);

            ApplicationUser user = userTask.Result;
            ApplicationUser ghostUser = ghostUserTask.Result;
            if (user == null)
            {
                return false;
            }

            var updateTasks = new List<Task>
            {
                UpdateUserCommentsAsync(user, ghostUser),
                UpdateUserCollectionsAsync(user, ghostUser),
                UpdateUserFollowersAsync(user),
                UpdateUserFollowingsAsync(user)
            };

            await Task.WhenAll(updateTasks);
            return await RemoveAsync(user);
        }


        private async Task UpdateUserCommentsAsync(ApplicationUser user, ApplicationUser ghostUser)
        {
            _logger.Log("Fetching the User's comments", LogTypes.DEBUG);
            IEnumerable<Comment> comments = await _commentRepository.GetByConditionAsync(c => c.Autor.Id == user.Id);
            if (comments.Any())
            {
                _logger.Log("Changing the comments author to a ghostUser", LogTypes.DEBUG);
                Parallel.ForEach(comments, comment =>
                {
                    comment.Autor = ghostUser;
                });
            }
        }

        private async Task UpdateUserCollectionsAsync(ApplicationUser user, ApplicationUser ghostUser)
        {
            await Task.Run(() =>
            {
                _logger.Log("Changing the collections author to a ghostUser", LogTypes.DEBUG);
                foreach (var collection in user.Collections)
                {
                    collection.ApplicationUser = ghostUser;
                }
            });
        }

        private async Task UpdateUserFollowersAsync(ApplicationUser user)
        {
            await Task.Run(() =>
            {
                _logger.Log("Updating followers counts", LogTypes.DEBUG);
                foreach (var follower in user.Followers)
                {
                    follower.FollowingCount -= 1;
                }
            });
        }

        private async Task UpdateUserFollowingsAsync(ApplicationUser user)
        {
            await Task.Run(() =>
            {
                _logger.Log("Updating following counts", LogTypes.DEBUG);
                foreach (var following in user.Following)
                {
                    following.FollowersCount -= 1;
                }
            });
        }
    }
}
