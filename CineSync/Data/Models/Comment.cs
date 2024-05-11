using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{

    public class Comment
    {
        public uint Id { get; set; }
        [Required]
        public ApplicationUser? Autor { get; set; }

        public long NumberOfLikes { get; set; } = 0;

        public long NumberOfDislikes { get; set; } = 0;

        public DateTime TimeStamp { get; set; } = DateTime.Now;

        [Required]
        public string? Content { get; set; }

        public ICollection<CommentAttachment>? Attachements { get; set; }

    }
}
