using CineSync.Utils.Adapters.ApiAdapters;

namespace CineSync.Controllers.MovieEndpoint
{
    public class ApiSearchResponse
    {
        public List<MovieSearchAdapter>? Results { get; set; }
    }
}
