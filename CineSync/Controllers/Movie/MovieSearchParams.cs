namespace CineSync.Controllers.Movie
{
    public class MovieSearchParameters
    {
        public string Query { get; set; }
        public string Language { get; set; } = "en-US"; // Default value
        public int? Page { get; set; }
        public bool IncludeAdult { get; set; } = false; // Default value
    }
}
