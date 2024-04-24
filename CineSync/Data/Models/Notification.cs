using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class Notification : Item
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string? NotificationType { get; set; }

        [Required]
        public Comment? Comment { get; set; }

        public ICollection<UsersNotifications>? UsersTonotify { get; set; }

    }
}
