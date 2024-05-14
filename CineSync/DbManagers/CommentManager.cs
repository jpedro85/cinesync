using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Core.Logger.Enums;
using Microsoft.Identity.Client;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages database operations related to comments, including creating, updating,
    /// and deleting comment data, and managing comment-related interactions like likes and attachments.
    /// </summary>
	public class CommentManager : DbManager<Comment>
    {
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly IRepositoryAsync<ApplicationUser> _userRepository;
        private readonly IRepositoryAsync<Comment> _commentRepository;

        /// <summary>
        /// Initializes a new instance of the CommentManager class, setting up repositories for movie and user entities.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for handling transactional operations.</param>
        /// <param name="logger">The logger for recording operational logs.</param>
        public CommentManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _movieRepository = _unitOfWork.GetRepositoryAsync<Movie>();
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
            _commentRepository = _unitOfWork.GetRepositoryAsync<Comment>();
        }

        /// <summary>
        /// Return the comment of a movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie to which the comment is being added.</param>
        /// <returns>Return the comment of a movie</returns>
        public async Task<ICollection<Comment>> GetCommentsOfMovie(int movieId) 
        {
            Movie movie = await _movieRepository.GetFirstByConditionAsync(movie => movie.MovieId == movieId, "Comments");

            ICollection<Comment> allcomments = new List<Comment>(0);
            foreach (var item in movie.Comments)
            {
                allcomments.Add( await _commentRepository.GetFirstByConditionAsync(c => c.Id == item.Id, "Autor") );
            }

            return movie.Comments;
        }

        /// <summary>
        /// Adds a comment to a specific movie by a specified user.
        /// </summary>
        /// <param name="comment">The comment entity to add.</param>
        /// <param name="movieId">The ID of the movie to which the comment is being added.</param>
        /// <param name="userId">The ID of the user adding the comment.</param>
        /// <returns>True if the comment is successfully added, otherwise false.</returns>
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
        /// Increments the number of likes on a given comment.
        /// </summary>
        /// <param name="comment">The comment to be liked.</param>
        public async Task AddLikeAsync(Comment comment)
        {
            comment.NumberOfLikes++;
            await _unitOfWork.SaveChangesAsync();
        }

		/// <summary>
		/// Increments the number of deslikes on a given comment.
		/// </summary>
		/// <param name="comment">The comment to be liked.</param>
		public async Task AddDesLikeAsync(Comment comment)
		{
            comment.NumberOfDislikes++;
			await _unitOfWork.SaveChangesAsync();
		}

		/// <summary>
		/// Decrements the number of likes on a given comment, ensuring it does not drop below zero.
		/// </summary>
		/// <param name="comment">The comment from which to remove a like.</param>
		public async Task RemoveLikeAsync(Comment comment)
        {
            comment.NumberOfLikes = comment.NumberOfLikes > 0 ? comment.NumberOfLikes - 1 : comment.NumberOfLikes;
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Adds an attachment to a specific comment.
        /// </summary>
        /// <param name="comment">The comment to add the attachment to.</param>
        /// <param name="attachment">The attachment to be added.</param>
        public async Task AddAttachmentAsync(Comment comment, CommentAttachment attachment)
        {
            if (comment.Attachements != null && !comment.Attachements.Contains(attachment))
            {
                comment.Attachements.Add(attachment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes an attachment from a specific comment.
        /// </summary>
        /// <param name="comment">The comment from which the attachment will be removed.</param>
        /// <param name="attachment">The attachment to remove.</param>
        public async Task RemoveAttachmentAsync(Comment discussion, CommentAttachment attachment)
        {
            if (discussion.Attachements != null && discussion.Attachements.Contains(attachment))
            {
                discussion.Attachements.Remove(attachment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates the content of a specific comment.
        /// </summary>
        /// <param name="comment">The comment to update.</param>
        /// <param name="content">The new content to set for the comment.</param>
        public async Task EditContentAsync(Comment comment, string content)
        {
            comment.Content = content;
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
