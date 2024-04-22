using CineSync.Data;
using CineSync.Data.Models;
using CineSync.Utils.Logger;

namespace CineSync.DbManagers
{
	public class DiscussionManager( ApplicationDbContext dbContext , ILoggerStrategy logger) : DbManager<Discussion>( dbContext, logger )
	{
		public async Task AddCommentAsync( Discussion discussion, Comment comment )
		{
			if( discussion.Comments != null && !discussion.Comments.Contains( comment ) )
			{
				discussion.Comments.Add( comment );
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task RemoveLikeAsync( Discussion discussion, Comment comment )
		{
			if (discussion.Comments != null && discussion.Comments.Contains( comment ))
			{
				discussion.Comments.Remove( comment );
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task AddLikeAsync( Discussion disussion )
		{
			disussion.NumberOfLikes++;
			await _dbContext.SaveChangesAsync();
		}

		public async Task RemoveLikeAsync( Discussion discussion )
		{
			discussion.NumberOfLikes = discussion.NumberOfLikes > 0 ? discussion.NumberOfLikes - 1 : discussion.NumberOfLikes;
			await _dbContext.SaveChangesAsync();
		}
	}
}
