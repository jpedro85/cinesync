using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
	public class CommentManager(ApplicationDbContext dbContext) : DbManager<Comment>(dbContext)
	{
		
		public async Task AddLikeAsync( Comment comment)
		{
			comment.NumberOfLikes++;
			await DbContext.SaveChangesAsync();
		}

		public async Task RemoveLikeAsync(Comment comment)
		{
			comment.NumberOfLikes = comment.NumberOfLikes > 0 ? comment.NumberOfLikes - 1 : comment.NumberOfLikes;
			await DbContext.SaveChangesAsync();
		}

		public async Task AddAttachmentAsync( Comment comment, CommentAttachment attachment )
		{
			if (comment.Attachements != null && !comment.Attachements.Contains(attachment) )
			{
				comment.Attachements.Add( attachment );
				await DbContext.SaveChangesAsync();
			}
		}

		public async Task RemoveAttachmentAsync( Comment discussion, CommentAttachment attachment )
		{
			if (discussion.Attachements != null && discussion.Attachements.Contains(attachment))
			{
				discussion.Attachements.Remove(attachment);
				await DbContext.SaveChangesAsync();
			}
		}

		public async Task EditContentAsync(Comment comment, string content)
		{
			comment.Content = content;
			await DbContext.SaveChangesAsync();
		}

	}
}
