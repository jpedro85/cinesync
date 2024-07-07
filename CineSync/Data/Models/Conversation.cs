using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class Conversation
    {
        public uint Id { get; set; }

        public string Name { get; set; } = default!;

        public bool IsGroupConversation { get; set; } = false;

        public ICollection<UserConversations> Participants { get; set; } = default!;

		public ICollection<Invite> Invites { get; set; } = new List<Invite>(0);

        public ICollection<Message> Messages { get; set; } = new List<Message>(0);

		public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Conversation))
            {
                return false;
            }

            return ((Conversation)obj).Id == this.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
