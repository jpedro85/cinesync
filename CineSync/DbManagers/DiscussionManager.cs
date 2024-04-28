using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages operations for the Discussion entities in the database.
    /// </summary>
	public class DiscussionManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : DbManager<Discussion>(unitOfWork, logger)
    {
        /// <summary>
        /// Adds a comment to a specific discussion if it does not already exist in the discussion's comment collection.
        /// </summary>
        /// <param name="discussion">The discussion to which the comment will be added.</param>
        /// <param name="comment">The comment to add to the discussion.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
        public async Task AddCommentAsync(Discussion discussion, Comment comment)
        {
            if (discussion.Comments != null && !discussion.Comments.Contains(comment))
            {
                discussion.Comments.Add(comment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes a specific comment from a discussion if it exists in the discussion's comments collection.
        /// </summary>
        /// <param name="discussion">The discussion from which the comment will be removed.</param>
        /// <param name="comment">The comment to remove from the discussion.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
        public async Task RemoveLikeAsync(Discussion discussion, Comment comment)
        {
            if (discussion.Comments != null && discussion.Comments.Contains(comment))
            {
                discussion.Comments.Remove(comment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Increments the like count of a discussion.
        /// </summary>
        /// <param name="discussion">The discussion whose like count will be incremented.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddLikeAsync(Discussion disussion)
        {
            disussion.NumberOfLikes++;
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Decrements the like count of a discussion if it is greater than zero.
        /// </summary>
        /// <param name="discussion">The discussion whose like count will be decremented.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RemoveLikeAsync(Discussion discussion)
        {
            discussion.NumberOfLikes = discussion.NumberOfLikes > 0 ? discussion.NumberOfLikes - 1 : discussion.NumberOfLikes;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
