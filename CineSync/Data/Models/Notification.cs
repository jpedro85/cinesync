using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineSync.Data.Models
{
    public class Notification
    {
        public uint Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string? NotificationType {  get; set; }

        [Required]
        public Comment? Comment { get; set; }

        public ICollection<UsersNotifications>? UsersTonotify { get; set; }

    }
}
