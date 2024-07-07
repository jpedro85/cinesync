using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class UserConversations
    {
        [Key]
        public uint Id { get; set; }

        public uint ConversationId { get; set; }

        public string UserId { get; set; }

        public Conversation Conversation { get; set; }

        public ApplicationUser User { get; set; }
    }
}
