namespace CineSync.Data.Models
{
    public class FollowedCollection
    {
        public uint Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public uint MovieCollectionId { get; set; }
        public MovieCollection MovieCollection { get; set; }
    }
}
