using System.Text;

namespace CineSync.Utils.Adapters.ApiAdapters
{
    public class MovieSearchAdapter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? PosterPath { get; set; }
        public byte[]? PosterImage { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Movie ID: {Id}");
            sb.AppendLine($"Movie Title: {Title}");

            return sb.ToString();
        }
    }
}
