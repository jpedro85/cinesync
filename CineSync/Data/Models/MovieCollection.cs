using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CineSync.Data.Models
{
    public class MovieCollection
    {
        public uint Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public bool IsPublic { get; set; } = false;

        [JsonIgnore]
        public ICollection<CollectionsMovies>? CollectionMovies { get; set; }

        [JsonIgnore]
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<FollowedCollection> FollowedCollections { get; set; }

        protected bool Equals(MovieCollection other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MovieCollection)obj);
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }
    }
}
