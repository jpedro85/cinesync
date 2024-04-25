using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
	public class DiscussionManager( IUnitOfWorkAsync unitOfWork  , ILoggerStrategy logger) : DbManager<Discussion>( unitOfWork, logger )
	{
		public async Task AddCommentAsync( Discussion discussion, Comment comment )
		{
			if( discussion.Comments != null && !discussion.Comments.Contains( comment ) )
			{
				discussion.Comments.Add( comment );
				await _unitOfWork.SaveChangesAsync();
			}
		}

		public async Task RemoveLikeAsync( Discussion discussion, Comment comment )
		{
			if (discussion.Comments != null && discussion.Comments.Contains( comment ))
			{
				discussion.Comments.Remove( comment );
				await _unitOfWork.SaveChangesAsync();
			}
		}

		public async Task AddLikeAsync( Discussion disussion )
		{
			disussion.NumberOfLikes++;
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task RemoveLikeAsync( Discussion discussion )
		{
			discussion.NumberOfLikes = discussion.NumberOfLikes > 0 ? discussion.NumberOfLikes - 1 : discussion.NumberOfLikes;
			await _unitOfWork.SaveChangesAsync();
		}
	}
}
