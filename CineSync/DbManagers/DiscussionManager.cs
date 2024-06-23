using CineSync.Components.Discussions;
using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages operations for the Discussion entities in the database.
    /// </summary>
	public class DiscussionManager : DbManager<Discussion>
    {

        private readonly IRepositoryAsync<ApplicationUser> _userRepository;
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly IRepositoryAsync<Comment> _commentRepository;

        /// <summary>
        /// Initializes a new instance of the CommentManager class, setting up repositories for movie and user entities.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for handling transactional operations.</param>
        /// <param name="logger">The logger for recording operational logs.</param>
        public DiscussionManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
            _movieRepository = _unitOfWork.GetRepositoryAsync<Movie>();
            _commentRepository = _unitOfWork.GetRepositoryAsync<Comment>();
        }


		/// <summary>
		/// Return the comment of a movie.
		/// </summary>
		/// <param name="movieId">The ID of the movie to which the comment is being added.</param>
		/// <returns>Return the comment of a movie</returns>
		public async Task<ICollection<Discussion>> GetDiscussionsOfMovie(int movieId)
		{
            Movie? movie = await _movieRepository.GetFirstByConditionAsync( movie => movie.MovieId == movieId, "Discussions");

            if (movie == null)
                return new List<Discussion>(0);
            else if (movie!.Discussions == null)
                return new List<Discussion>(0);
            else
                return movie.Discussions ;
		}




		/// <summary>
		/// Adds a discussion to a specific movie by a specified user.
		/// </summary>
		/// <param name="comment">The comment entity to add.</param>
		/// <param name="movieId">The ID of the movie to which the comment is being added.</param>
		/// <param name="userId">The ID of the user adding the comment.</param>
		/// <returns>True if the comment is successfully added, otherwise false.</returns>
		public async Task<bool> AddDiscussion(Discussion discussion, int movieId, string userId)
        {
            Movie? movie = await _movieRepository.GetFirstByConditionAsync(movie => movie.MovieId == movieId, "Discussions");
            ApplicationUser? user = await _userRepository.GetFirstByConditionAsync(user => user.Id == userId);

            if (movie == null || user == null)
                return false;

            discussion.Autor = user;

            if (movie.Discussions == null)
                movie.Discussions = new List<Discussion>();

            movie.Discussions.Add(discussion);

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Increments the number of likes on a given discussion.
        /// </summary>
        /// <param name="discussion">The discussion to be liked.</param>
        public async Task<bool> AddLikeAsync(Discussion discussion, string userId)
        {
            ApplicationUser? user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "LikedDiscussions");

            if (user == null)
                return false;

            if (user.LikedDiscussions == null)
                user.LikedDiscussions = new List<UserLikedDiscussion>();

            user.LikedDiscussions.Add(
                    new UserLikedDiscussion()
                    {
                        Discussion = discussion,
                        User = user,
                        UserId = user.Id,
                        DiscussionId = discussion.Id
                    }
                );

            discussion.NumberOfLikes++;

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Decrements the number of likes on a given discussion.
        /// </summary>
        /// <param name="discussion">The discussion to remove the like.</param>
        public async Task<bool> RemoveLikeAsync(Discussion discussion, string userId)
        {
            ApplicationUser? user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "LikedDiscussions");

            if (user == null || user.LikedDiscussions == null)
                return false;

            user.LikedDiscussions = user.LikedDiscussions.Where(u => u.UserId == userId && u.DiscussionId != discussion.Id).ToList();
            discussion.NumberOfLikes = user.LikedDiscussions.Count;

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Increments the number of deslikes on a given discussion.
        /// </summary>
        /// <param name="discussion">The discussion to be disliked.</param>
        public async Task<bool> AddDesLikeAsync(Discussion discussion, string userId)
        {
            ApplicationUser? user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "DislikedDiscussions");

            if (user == null)
                return false;

            if (user.DislikedDiscussions == null)
                user.DislikedDiscussions = new List<UserDislikedDiscussion>();

            user.DislikedDiscussions.Add(
                    new UserDislikedDiscussion()
                    {
                        Discussion = discussion,
                        User = user,
                        UserId = user.Id,
                        DiscussionId = discussion.Id,
                    }
                );

            discussion.NumberOfDeslikes++;

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Decrements the number of deslikes on a given discussion.
        /// </summary>
        /// <param name="discussion">The discussion to remove the dislike.</param>
        public async Task<bool> RemoveDesLikeAsync(Discussion discussion, string userId)
        {
            ApplicationUser? user = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId, "DislikedDiscussions");

            if (user == null || user.DislikedDiscussions == null)
                return false;

            user.DislikedDiscussions = user.DislikedDiscussions.Where(u => u.UserId == userId && u.DiscussionId != discussion.Id).ToList();
            discussion.NumberOfDeslikes = user.DislikedDiscussions.Count;

            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the Discussion .
        /// </summary>
        /// <param name="editedDiscussion">The Discussion to update.</param>
        public async Task<bool> EditAsync(Discussion editedDiscussion)
        {
            Discussion? discussion = await _repository.GetFirstByConditionAsync(ed => ed.Equals(editedDiscussion));

            if (discussion == null)
                return false;

            discussion.Title = editedDiscussion.Title;
            discussion.HasSpoiler = editedDiscussion.HasSpoiler;

            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
