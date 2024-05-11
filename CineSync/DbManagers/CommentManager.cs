using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Core.Logger.Enums;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages database operations for comment-related actions such as adding likes, managing attachments, and editing comment content.
    /// </summary>
	public class CommentManager : DbManager<Comment>
    {
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly IRepositoryAsync<ApplicationUser> _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentManager"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database transactions.</param>
        /// <param name="logger">The logger for logging messages.</param>
        public CommentManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _movieRepository = _unitOfWork.GetRepositoryAsync<Movie>();
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
        }

        public async Task<bool> AddComment(Comment comment, int movieId, string userId)
        {
            Movie movie = await _movieRepository.GetFirstByConditionAsync(movie => movie.MovieId == movieId, "Comments");
            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId);

            if (movie == null)
            {
                _logger.Log("Movie not found.", LogTypes.WARN);
                return false;
            }

            if (user == null)
            {
                _logger.Log("Invalid userId", LogTypes.WARN);
                return false;
            }
            comment.Autor = user;

            if (movie.Comments == null)
                movie.Comments = new List<Comment>();

            movie.Comments.Add(comment);

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Increments the number of likes on a comment.
        /// </summary>
        /// <param name="comment">The comment to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddLikeAsync(Comment comment)
        {
            comment.NumberOfLikes++;
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Decrements the number of likes on a comment if greater than zero.
        /// </summary>
        /// <param name="comment">The comment to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveLikeAsync(Comment comment)
        {
            comment.NumberOfLikes = comment.NumberOfLikes > 0 ? comment.NumberOfLikes - 1 : comment.NumberOfLikes;
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Adds an attachment to a comment.
        /// </summary>
        /// <param name="comment">The comment to which the attachment is added.</param>
        /// <param name="attachment">The attachment to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAttachmentAsync(Comment comment, CommentAttachment attachment)
        {
            if (comment.Attachements != null && !comment.Attachements.Contains(attachment))
            {
                comment.Attachements.Add(attachment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes an attachment from a comment.
        /// </summary>
        /// <param name="comment">The comment from which the attachment is removed.</param>
        /// <param name="attachment">The attachment to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveAttachmentAsync(Comment discussion, CommentAttachment attachment)
        {
            if (discussion.Attachements != null && discussion.Attachements.Contains(attachment))
            {
                discussion.Attachements.Remove(attachment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates the content of a comment.
        /// </summary>
        /// <param name="comment">The comment to update.</param>
        /// <param name="content">The new content for the comment.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task EditContentAsync(Comment comment, string content)
        {
            comment.Content = content;
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
