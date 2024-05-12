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
    }
}
