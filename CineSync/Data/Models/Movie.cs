using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineSync.Data.Models
{
    public class Movie
    {
        public uint Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        public float RatingCS { get; set; } = 0;

        [Column(TypeName = "decimal(3,1)")]
        public float RatingIMDB { get; set; } = 0;

        [StringLength(30)]
        public string? Gender { get; set; }

        public byte[]? Poster { get; set; }

        public string? Sumary { get; set; }

        public DateTime ReleaseDate { get; set; }

        public ICollection<String>? Cast { get; set; }

        public String? TrailerLink { get; set; }

        public String? Director { get; set; }

        public String? Awards { get; set; }

        public float? Duration { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public ICollection<Discussion>? Discutions { get; set; }

    }
}
