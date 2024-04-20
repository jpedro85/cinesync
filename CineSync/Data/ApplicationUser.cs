using CineSync.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace CineSync.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public byte[]? ProfileImage { get; set; }

        public ICollection<ApplicationUser>? Followers { get; set; }

        public ICollection<ApplicationUser>? Following { get; set; }

        public float WatchTime { get; set; } = 0;

        public ICollection<MovieCollection>? Collections { get; set; }

        public ICollection<UsersNotifications>? Notifications { get; set; }

    }

}
