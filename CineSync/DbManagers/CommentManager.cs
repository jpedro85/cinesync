using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.Core.Logger.Enums;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages database operations related to comments, including creating, updating,
    /// and deleting comment data, and managing comment-related interactions like likes and attachments.
    /// </summary>
	public class CommentManager : DbManager<Comment>
    {
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly IRepositoryAsync<Discussion> _discussionRepository;
        private readonly IRepositoryAsync<ApplicationUser> _userRepository;


        /// <summary>
        /// Initializes a new instance of the CommentManager class, setting up repositories for movie and user entities.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for handling transactional operations.</param>
        /// <param name="logger">The logger for recording operational logs.</param>
        public CommentManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _movieRepository = _unitOfWork.GetRepositoryAsync<Movie>();
            _discussionRepository = _unitOfWork.GetRepositoryAsync<Discussion>();
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
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
                allcomments.Add(await _repository.GetFirstByConditionAsync(c => c.Id == item.Id, "Autor.UserImage", "Attachements"));
            }

            return movie.Comments;
        }

        /// <summary>
        /// Return the comment of a movie.
        /// </summary>
        /// <param name="movieId">The ID of the movie to which the comment is being added.</param>
        /// <returns>Return the comment of a movie</returns>
        public async Task<ICollection<Comment>> GetCommentsOfDiscussion(uint discussionId)
        {
            Discussion? discussion = await _discussionRepository.GetFirstByConditionAsync( discussion => discussion.Id == discussionId, "Comments", "Comments.Attachements");

            if ( discussion != null && discussion.Comments != null)
                return discussion.Comments;

            return new List<Comment>(0);
        }

        /// <summary>
        /// Adds a comment to a specific movie by a specified user.
        /// </summary>
        /// <param name="comment">The comment entity to add.</param>
        /// <param name="movieId">The ID of the movie to which the comment is being added.</param>
        /// <param name="userId">The ID of the user adding the comment.</param>
        /// <returns>True if the comment is successfully added, otherwise false.</returns>
        public async Task<bool> AddCommentToMovie(Comment comment, int movieId, string userId)
        {
            Movie? movie = await _movieRepository.GetFirstByConditionAsync(movie => movie.MovieId == movieId, "Comments");
            ApplicationUser? user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId);

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
        /// Adds a comment to a specific movie by a specified user.
        /// </summary>
        /// <param name="comment">The comment entity to add.</param>
        /// <param name="movieId">The ID of the movie to which the comment is being added.</param>
        /// <param name="userId">The ID of the user adding the comment.</param>
        /// <returns>True if the comment is successfully added, otherwise false.</returns>
        public async Task<bool> AddCommentToDiscussion(Comment comment, uint discussionId, string userId)
        {
            Discussion? discussion = await _discussionRepository.GetFirstByConditionAsync( d => d.Id == discussionId, "Comments");
            ApplicationUser? user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId);

            if (discussion == null || user == null)
                return false;
            
            Console.WriteLine($" d:{discussionId} u:{userId}");
            comment.Autor = user;

            if (discussion.Comments == null)
            {
                discussion.Comments = new List<Comment>() { comment };
                return await _unitOfWork.SaveChangesAsync();
            }
            else if (!discussion.Comments.Contains(comment))
            {
                discussion.Comments.Add(comment);
                return await _unitOfWork.SaveChangesAsync();
            }
            else
                return false;

        }

        /// <summary>
        /// Increments the number of likes on a given comment.
        /// </summary>
        /// <param name="comment">The comment to be liked.</param>
        public async Task<bool> AddLikeAsync(Comment comment, string userId)
        {
            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "LikedComments" );

            if(user == null)
                return false;


            if (user.LikedComments == null)
                user.LikedComments = new List<UserLikedComment>();

            user.LikedComments.Add(
                    new UserLikedComment()
                    {
                        Comment = comment,
                        User = user,
                        UserId = user.Id,
                        CommentId = comment.Id
                    }
                );

            comment.NumberOfLikes++;

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Decrements the number of likes on a given comment.
        /// </summary>
        /// <param name="comment">The comment to be liked.</param>
        public async Task<bool> RemoveLikeAsync(Comment comment, string userId)
        {
            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "LikedComments" );

            if (user == null || user.LikedComments == null)
                return false;

            user.LikedComments = user.LikedComments.Where(u => u.UserId == userId && u.CommentId != comment.Id).ToList();
            comment.NumberOfLikes = user.LikedComments.Count;

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Increments the number of deslikes on a given comment.
        /// </summary>
        /// <param name="comment">The comment to be liked.</param>
        public async Task<bool> AddDesLikeAsync(Comment comment, string userId)
        {
            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "DislikedComments");

            if (user == null)
                return false;

            if (user.DislikedComments == null)
                user.DislikedComments = new List<UserDislikedComment>();

            user.DislikedComments.Add(
                    new UserDislikedComment()
                    {
                        Comment = comment,
                        User = user,
                        UserId = user.Id,
                        CommentId = comment.Id,
                    }
                );

            comment.NumberOfDislikes++;

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Decrements the number of deslikes on a given comment.
        /// </summary>
        /// <param name="comment">The comment to be liked.</param>
        public async Task<bool> RemoveDesLikeAsync(Comment comment, string userId)
        {
            ApplicationUser user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "DislikedComments");

            if (user == null || user.DislikedComments == null)
                return false;

            user.DislikedComments = user.DislikedComments.Where(u => u.UserId == userId && u.CommentId != comment.Id).ToList();
            comment.NumberOfDislikes = user.DislikedComments.Count;

            return await _unitOfWork.SaveChangesAsync();
        }

        // NOTE: Need to check if it actually searches the Attachment as its not a database object
        /// <summary>
        /// Adds an attachment to a specific comment.
        /// </summary>
        /// <param name="comment">The comment to add the attachment to.</param>
        /// <param name="attachment">The attachment to be added.</param>
        public async Task<bool> AddAttachmentAsync(Comment comment, CommentAttachment attachment)
        {
            Comment Comment = await _repository.GetFirstByConditionAsync(c => c.Id == comment.Id, "Attachements");

            if (Comment == null || Comment.Attachements == null)
                return false;

            if (Comment.Attachements != null && !Comment.Attachements.Contains(attachment))
            {
                comment.Attachements.Add(attachment);
                return await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        // NOTE: Need to check if it actually searches the Attachment as its not a database object
        /// <summary>
        /// Removes an attachment from a specific comment.
        /// </summary>
        /// <param name="comment">The comment from which the attachment will be removed.</param>
        /// <param name="attachment">The attachment to remove.</param>
        public async Task<bool> RemoveAttachmentAsync(Comment comment, CommentAttachment attachment)
        {
            Comment Comment = await _repository.GetFirstByConditionAsync(c => c.Id == comment.Id, "Attachements");

            if (Comment == null || Comment.Attachements == null)
                return false;

            if (Comment.Attachements != null && !Comment.Attachements.Contains(attachment))
            {
                comment.Attachements.Remove(attachment);
                return await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        /// <summary>
        /// Updates the content of a specific comment.
        /// </summary>
        /// <param name="comment">The comment to update.</param>
        /// <param name="content">The new content to set for the comment.</param>
        public async Task<bool> EditAsync(Comment editedComment)
        {
            Comment? comment = await _repository.GetFirstByConditionAsync(ed => ed.Equals(editedComment));

            if (comment == null)
                return false;

            comment.Content = editedComment.Content;
            comment.HasSpoiler = editedComment.HasSpoiler;

            return await _unitOfWork.SaveChangesAsync();
        }

    }
}
