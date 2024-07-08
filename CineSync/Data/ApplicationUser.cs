using CineSync.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;

namespace CineSync.Data
{
    public class ApplicationUser : IdentityUser
    {
        public UserImage? UserImage { get; set; }

        [JsonIgnore]
        public ICollection<ApplicationUser>? Followers { get; set; }

        public uint FollowersCount { get; set; }

        [JsonIgnore]
        public ICollection<ApplicationUser>? Following { get; set; }

        public uint FollowingCount { get; set; }

        public float WatchTime { get; set; } = 0;

        public ICollection<MovieCollection>? Collections { get; set; }

        public ICollection<FollowedCollection> FollowedCollections { get; set; }

        public ICollection<UsersNotifications>? Notifications { get; set; }

        public bool Banned { get; set; }

        public bool Blocked { get; set; }

        [JsonIgnore]
        public ICollection<UserLikedComment> LikedComments { get; set; }

        [JsonIgnore]
        public ICollection<UserDislikedComment> DislikedComments { get; set; }

        [JsonIgnore]
        public ICollection<UserLikedDiscussion> LikedDiscussions { get; set; }

        [JsonIgnore]
        public ICollection<UserDislikedDiscussion> DislikedDiscussions { get; set; }

        [JsonIgnore]
        public ICollection<UserConversations> Conversations { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is ApplicationUser otherUser)
            {
                return this.Id == otherUser.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id != null ? this.Id.GetHashCode() : 0;
        }
    }

}
