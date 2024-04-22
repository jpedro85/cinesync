using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineSync.Data.Models
{
    public class Movie
    {
        public uint Id { get; set; }

        public int MovieId { get; set; }

        [Required]
        public string? Title { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        public float RatingCS { get; set; } = 0.0f;

        [Column(TypeName = "decimal(3,1)")]
        public float Rating { get; set; } = 0.0f;

        public ICollection<Genre> Genres { get; set; }

        public byte[] PosterImage { get; set; }

        public string? Overview { get; set; }

        public DateTime ReleaseDate { get; set; }

        public IList<string>? Cast { get; set; }

        public string? TrailerKey { get; set; }

        public string? Director { get; set; }

        public string? Awards { get; set; }

        public short RunTime { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public ICollection<Discussion>? Discutions { get; set; }

    }
}
