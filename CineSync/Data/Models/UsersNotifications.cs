using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class UsersNotifications : Item
    {
        public uint NotificationId { get; set; }

        [Required]
        public string? ApplicationUserId { get; set; }

        public Notification Notification { get; set; } = null!;

        public ApplicationUser AplicationUser { get; set; } = null!;

    }
}
