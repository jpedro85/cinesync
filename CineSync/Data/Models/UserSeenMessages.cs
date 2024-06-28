using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class UserSeenMessages
    {
        public uint Id { get; set; }

        public string UserId { get; set; } = default!;

        public uint MessageId {  get; set; }

        public ApplicationUser User { get; set; } = default!;

        public Message Message { get; set; } = default!;

    }
}
