using CineSync.Components.Comments;
using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{

    public class Comment
    {
        public uint Id { get; set; }

        public uint? MovieId { get; set; }

        public uint? DiscussionId { get; set; }

        [Required]
        public ApplicationUser Autor { get; set; }

        [Required]
        public string? Content { get; set; }

        public long NumberOfLikes { get; set; } = 0;

        public long NumberOfDislikes { get; set; } = 0;

        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public ICollection<CommentAttachment>? Attachements { get; set; }

        public ICollection<UserLikedComment> LikedByUsers { get; set; }

        public ICollection<UserDislikedComment> DislikedByUsers { get; set; }

        public bool HasSpoiler { get; set; } = false;

        public override bool Equals(object? obj)
        {

            if (obj == null || !(obj is Comment))
            {
                return false;
            }

            return ((Comment)obj).Id == this.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
