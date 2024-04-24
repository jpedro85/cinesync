using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class MovieCollection : Item
    {
        [Required]
        public string? Name { get; set; }

        public bool IsPublic { get; set; } = false;

        public ICollection<CollectionsMovies>? CollectionMovies { get; set; }
    }
}
