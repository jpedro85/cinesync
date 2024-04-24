using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class Genre : Item
    {
        public int? TmdbId { get; set; }

        [Required]
        public string? Name { get; set; }

        public ICollection<Movie>? Movies { get; set; }
    }
}
