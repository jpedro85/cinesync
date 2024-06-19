using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class UserDislikedDiscussion
    {
        [Key]
        public uint Id { get; set; }

        public uint DiscussionId { get; set; }

        public string UserId { get; set; }

        public Discussion Discussion { get; set; }

        public ApplicationUser User { get; set; }
    }
}
