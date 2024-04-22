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

        public uint VoteCount { get; set; }

        public override string ToString()
        {
            return $"Movie: \n" +
                   $"  Id: {Id}\n" +
                   $"  MovieId: {MovieId}\n" +
                   $"  Title: {Title}\n" +
                   $"  Genres: {(Genres != null ? string.Join(", ", Genres.Select(g => g.Name)) : "None")}\n" +
                   $"  Overview: {Overview}\n" +
                   $"  Release Date: {ReleaseDate.ToShortDateString()}\n" +
                   $"  Cast: {(Cast != null ? string.Join(", ", Cast) : "None")}\n" +
                   $"  Trailer Key: {TrailerKey}\n" +
                   $"  Director: {Director}\n" +
                   $"  Awards: {Awards}\n" +
                   $"  Run Time: {RunTime} minutes\n" +
                   $"  Comments: {(Comments != null ? Comments.Count() + " Comments" : "No Comments")}\n" +
                   $"  Discussions: {(Discutions != null ? Discutions.Count() + " Discussions" : "No Discussions")}";
        }
    }
}
