using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class Message
    {
        public uint Id { get; set; }

        public uint ConversationId { get; set; }

        public uint? ReplayMessageId{ get; set; }

        public Message? ReplayMessage { get; set; } = default!;

        [Required]
        public ApplicationUser Autor { get; set; } = default!;

        public string? Content { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public ICollection<UserSeenMessages>? SeenByUsers { get; set; } 

        public ICollection<MessageAttachement>? Attachements { get; set; }

        public ICollection<Reaction>? Reactions { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Message))
            {
                return false;
            }

            return ((Message)obj).Id == this.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
