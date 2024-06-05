using CineSync.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CineSync.Data
{
    public class ApplicationUser : IdentityUser
    {
        public UserImage? UserImage { get; set; }

        public ICollection<ApplicationUser>? Followers { get; set; }

        public uint FollowersCount { get; set; }

        public ICollection<ApplicationUser>? Following { get; set; }

        public uint FollowingCount { get; set; }

        public float WatchTime { get; set; } = 0;

        public ICollection<MovieCollection>? Collections { get; set; }

        public ICollection<UsersNotifications>? Notifications { get; set; }

        public bool Banned { get; set; }

        public bool Blocked { get; set; }

        public ICollection<UserLikedComment> LikedComments { get; set; }

        public ICollection<UserDislikedComment> DislikedComments { get; set; }
    }

}
