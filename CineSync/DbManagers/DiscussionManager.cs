using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
	public class DiscussionManager( ApplicationDbContext dbContext ) : DbManager<Discussion>( dbContext )
	{
		public async Task AddCommentAsync( Discussion discussion, Comment comment )
		{
			if( discussion.Comments != null && !discussion.Comments.Contains( comment ) )
			{
				discussion.Comments.Add( comment );
				await DbContext.SaveChangesAsync();
			}
		}

		public async Task RemoveLikeAsync( Discussion discussion, Comment comment )
		{
			if (discussion.Comments != null && discussion.Comments.Contains( comment ))
			{
				discussion.Comments.Remove( comment );
				await DbContext.SaveChangesAsync();
			}
		}

		public async Task AddLikeAsync( Discussion disussion )
		{
			disussion.NumberOfLikes++;
			await DbContext.SaveChangesAsync();
		}

		public async Task RemoveLikeAsync( Discussion discussion )
		{
			discussion.NumberOfLikes = discussion.NumberOfLikes > 0 ? discussion.NumberOfLikes - 1 : discussion.NumberOfLikes;
			await DbContext.SaveChangesAsync();
		}
	}
}
