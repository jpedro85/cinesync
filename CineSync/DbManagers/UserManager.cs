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
        private readonly IRepositoryAsync<Discussion> _discussionRepository;

        /// <summary>
        /// Initializes a new instance of the UserManager class.
        /// </summary>
        /// <param name="unitOfWork">Provides an interface for a unit of work mechanism to handle transactions.</param>
        /// <param name="logger">Provides logging capabilities to trace the operations and errors.</param>
        public UserManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _commentRepository = _unitOfWork.GetRepositoryAsync<Comment>();
            _discussionRepository = _unitOfWork.GetRepositoryAsync<Discussion>();
        }

        /// <summary>
        /// Attempts to change the username of a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier for the user whose username is to be changed.</param>
        /// <param name="username">The new username to be assigned to the user.</param>
        /// <returns>
        /// A task that represents the asynchronous operation, containing a boolean value.
        /// True if the username was successfully changed; otherwise, False if the user does not exist,
        /// the username is already taken, or the new username is null or empty.
        /// </returns>
        /// <remarks>
        /// This method first ensures the user exists and that the proposed new username is not null or already in use.
        /// If these conditions are met, it updates the username and commits the changes to the database.
        /// </remarks>
        public async Task<bool> ChangeUsernameAsync(string userId, string username)
        {
            _logger.Log("Checking if the Username is already in user", LogTypes.DEBUG);
            ApplicationUser user = await GetFirstByConditionAsync(u => u.Id == userId);
            if (user == null || string.IsNullOrEmpty(username) || await GetFirstByConditionAsync(user => user.UserName == username) != null)
            {
                _logger.Log("The Username is already in Use", LogTypes.WARN);
                return false;
            }

            user.UserName = username;
            _logger.Log("The Username has already been changed", LogTypes.WARN);
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Allows a user to follow another user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user who wants to follow.</param>
        /// <param name="userToFollowId">The unique identifier of the user to be followed.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean value indicating success or failure.</returns>
        public async Task<bool> Follow(string userId, string userToFollowId)
        {
            if (userId == userToFollowId)
                return false;

            _logger.Log($"User {userId} attempting to follow {userToFollowId}", LogTypes.DEBUG);
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
                _logger.Log($"User {userId} is now following {userToFollowId}", LogTypes.DEBUG);
            }
            else
            {
                _logger.Log($"User {userId} is already following {userToFollowId}", LogTypes.WARN);
                return false;
            }

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Allows a user to unfollow another user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user who wants to unfollow.</param>
        /// <param name="userToFollowId">The unique identifier of the user to be unfollowed.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean value indicating success or failure.</returns>
        public async Task<bool> UnFollow(string userId, string userToFollowId)
        {
            if (userId == userToFollowId)
                return false;

            _logger.Log($"User {userId} attempting to unfollow {userToFollowId}", LogTypes.DEBUG);
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

            _logger.Log($"User {userId} has unfollowed {userToFollowId}", LogTypes.DEBUG);
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Changes the profile picture of the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="image">The image data to be set as the profile picture.</param>
        /// <param name="contentType">The content type of the image data.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean value indicating success or failure.</returns>
        public async Task<bool> ChangeProfilePictureAsync(string userId, byte[] image, string contentType)
        {
            ApplicationUser user = await GetFirstByConditionAsync(u => u.Id == userId, "UserImage");
            if (user == null || image == null || contentType == null)
            {
                _logger.Log("User or image data is null", LogTypes.WARN);
                return false;
            }

            if (user.UserImage != null)
            {
                _logger.Log($"User {user.Id} already has an image. Updating existing image.", LogTypes.DEBUG);
                user.UserImage.ImageData = image;
                user.UserImage.ContentType = contentType;
                _logger.Log($"User {user.Id}'s profile image has been updated", LogTypes.DEBUG);
            }
            else
            {
                _logger.Log($"Creating a new profile image for user {user.Id}", LogTypes.DEBUG);
                UserImage userImage = new UserImage
                {
                    UserId = user.Id,
                    ImageData = image,
                    ContentType = contentType
                };
                _logger.Log("New profile image created for user", LogTypes.DEBUG);
                user.UserImage = userImage;
            }

            _logger.Log("Saving User Image changes to the database", LogTypes.WARN);
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the account of the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose account is to be deleted.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean value indicating success or failure.</returns>
        public async Task<bool> DeleteAccountAsync(string userId)
        {
            _logger.Log("Fetching the user and the ghost user", LogTypes.DEBUG);
            var userTask = GetFirstByConditionAsync(u => u.Id == userId, "Collections", "Followers", "Following");
            var ghostUserTask = GetFirstByConditionAsync(u => u.Id == "0");
            await Task.WhenAll(userTask, ghostUserTask);

            ApplicationUser user = userTask.Result;
            ApplicationUser ghostUser = ghostUserTask.Result;
            if (user == null || ghostUser == null)
            {
                _logger.Log("User or ghost user is null", LogTypes.WARN);
                return false;
            }

            var updateTasks = new List<Task>
            {
                UpdateUserCommentsAsync(user, ghostUser),
                UpdateUserDiscussionsAsync(user, ghostUser),
                UpdateUserCollectionsAsync(user, ghostUser),
                UpdateUserFollowersAsync(user),
                UpdateUserFollowingsAsync(user)
            };

            _logger.Log("Updating user references before deletion", LogTypes.DEBUG);
            await Task.WhenAll(updateTasks);
            _logger.Log("Removing user from database", LogTypes.WARN);
            return await RemoveAsync(user);
        }

        /// <summary>
        /// Updates the author of the user's comments to the ghost user.
        /// </summary>
        /// <param name="user">The user whose comments need to be updated.</param>
        /// <param name="ghostUser">The ghost user to set as the new author.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateUserCommentsAsync(ApplicationUser user, ApplicationUser ghostUser)
        {
            _logger.Log($"Fetching the user's {user.Id} comments", LogTypes.DEBUG);
            IEnumerable<Comment> comments = await _commentRepository.GetByConditionAsync(c => c.Autor.Id == user.Id);
            if (comments.Any())
            {
                _logger.Log($"Changing the comments author to the ghost user", LogTypes.DEBUG);
                Parallel.ForEach(comments, comment =>
                {
                    comment.Autor = ghostUser;
                    _logger.Log($"The comment {comment.Id} has changed author from {user.Id} to ghostUser", LogTypes.DEBUG);
                });
            }
        }

        /// <summary>
        /// Updates the author of the user's collections to the ghost user.
        /// </summary>
        /// <param name="user">The user whose collections need to be updated.</param>
        /// <param name="ghostUser">The ghost user to set as the new author.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateUserCollectionsAsync(ApplicationUser user, ApplicationUser ghostUser)
        {
            await Task.Run(() =>
            {
                _logger.Log($"Changing the collections author {user.Id} to the ghost user", LogTypes.DEBUG);
                foreach (var collection in user.Collections)
                {
                    collection.ApplicationUser = ghostUser;
                    _logger.Log($"The collection {collection.Id} has change author from {user.Id} to ghostUser", LogTypes.DEBUG);
                }
            });
        }

        /// <summary>
        /// Updates the follower counts for users who were following the deleted user.
        /// </summary>
        /// <param name="user">The user whose followers need to be updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateUserFollowersAsync(ApplicationUser user)
        {
            await Task.Run(() =>
            {
                _logger.Log("Updating followers counts", LogTypes.DEBUG);
                foreach (var follower in user.Followers)
                {
                    if (follower.FollowingCount > 0)
                        follower.FollowingCount -= 1;

                    _logger.Log($"The user {follower.Id} following count has been updated", LogTypes.DEBUG);
                }
            });
        }

        /// <summary>
        /// Updates the following counts for users who were being followed by the deleted user.
        /// </summary>
        /// <param name="user">The user whose followings need to be updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateUserFollowingsAsync(ApplicationUser user)
        {
            await Task.Run(() =>
            {
                _logger.Log("Updating following counts", LogTypes.DEBUG);
                foreach (var following in user.Following!)
                {
                    if (following.FollowersCount > 0)
                        following.FollowersCount -= 1;

                    _logger.Log($"The user {following.Id} follower count has been updated", LogTypes.DEBUG);
                }
            });
        }

        /// <summary>
        /// Updates the author of the user's discussions to the ghost user.
        /// </summary>
        /// <param name="user">The user whose discussions need to be updated.</param>
        /// <param name="ghostUser">The ghost user to set as the new author.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateUserDiscussionsAsync(ApplicationUser user, ApplicationUser ghostUser)
        {
            _logger.Log($"Fetching the User's {user.Id} discussions", LogTypes.DEBUG);
            IEnumerable<Discussion> discussions = await _discussionRepository.GetByConditionAsync(d => d.Autor.Id == user.Id);
            if (discussions.Any())
            {
                _logger.Log("Changing the discussion author to a ghostUser", LogTypes.DEBUG);
                Parallel.ForEach(discussions, discussion =>
                {
                    discussion.Autor = ghostUser;
                    _logger.Log($"The discussion {discussion.Id} has changed author from {user.Id} to ghostUser", LogTypes.DEBUG);
                });
            }
        }


    }
}
