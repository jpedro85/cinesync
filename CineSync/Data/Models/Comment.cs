using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{

    public class Comment : Item
    {
        [Required]
        public ApplicationUser? Autor { get; set; }

        public long NumberOfLikes { get; set; } = 0;

        public long NumberOfDeslikes { get; set; } = 0;

        [Required]
        public string? Content { get; set; }

        public ICollection<CommentAttachment>? Attachements { get; set; }

    }
}
