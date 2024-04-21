namespace CineSync.Controllers.MovieEndpoint
{
    public class MovieSearchParameters
    {
        public string Query { get; set; }
        public string Language { get; set; } = "en-US";
        public int? Page { get; set; }
        public bool IncludeAdult { get; set; } = false;
    }
}
