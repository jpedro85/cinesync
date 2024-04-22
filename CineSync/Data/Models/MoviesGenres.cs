using System.ComponentModel.DataAnnotations;

namespace CineSync.Data.Models
{
    public class MoviesGenres
    {
        public int MovieId { get; set; }

        [Required]
        public int GenreId  { get; set; }

        public Genre Genre { get; set; }

        public Movie Movie { get; set; }
    }
}
