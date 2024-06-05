using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class UserLikedComment
    {
        [Key]
        public uint Id { get; set; }

        public uint CommentId { get; set; }

        public string UserId { get; set; }

        public Comment Comment { get; set; }

        public ApplicationUser User { get; set; }
    }
}
