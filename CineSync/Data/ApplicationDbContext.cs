using CineSync.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineSync.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Movie> Movies { get; set; }

        public DbSet<MovieCollection> Collections { get; set; }

        public DbSet<Discussion> Discutions { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentAttachment> CommentAttachments { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<UserLikedComment> UserLikedComments { get; set; }

        public DbSet<UserDislikedComment> UserDislikedComments { get; set; }

        public DbSet<UserLikedDiscussion> UserLikedDiscussion { get; set; }

        public DbSet<UserDislikedDiscussion> UserDislikedDiscussion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=CineSyncData.db");
        }
    }
}
