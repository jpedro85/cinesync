using CineSync.Utils.Logger;
using CineSync.Utils.Logger.Enums;
using CineSync.Utils.Adapters.ApiAdapters;
using CineSync.Utils.Adapters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CineSync.Controllers.Movie
{
    // TODO: Add the functionality to search first on our DB
    [Route("movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ApiService _apiService;
        private readonly ILoggerStrategy _logger;

        public MovieController(ApiService apiService, ILoggerStrategy logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // TODO: Check our database first if there is a movie if not fetch from api and upload to the database and send the response we got
        [HttpGet]
        public async Task<IActionResult> GetMovieById([FromQuery] string id)
        {
            string endpoint = $"movie/{id}?append_to_response=credits,videos";
            _logger.Log($"Fetching the Movie details {id}", LogTypes.INFO);
            string data = await _apiService.FetchDataAsync(endpoint);
            var rawResponse = JsonConvert.DeserializeObject<dynamic>(data);
            IMovie movie = await MovieDetailsAdapter.FromJson(rawResponse);
            return Ok(movie);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetQueryResults([FromQuery] MovieSearchParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Query))
            {
                return BadRequest("Query and API key are required parameters.");
            }

            string queryString = $"query={Uri.EscapeDataString(parameters.Query)}&language={parameters.Language}&include_adult={parameters.IncludeAdult.ToString().ToLower()}";

            if (parameters.Page.HasValue)
            {
                queryString += $"&page={parameters.Page.Value}";
            }

            string endpoint = $"search/movie?{queryString}";

            _logger.Log($"Fetching the query results for {parameters.Query}", LogTypes.INFO);
            string data = await _apiService.FetchDataAsync(endpoint);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new MovieConverter());

            ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(data, settings);

            return Ok(apiResponse);
        }


        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies([FromQuery] string? page)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>
            {
                ["language"] = "en-US",
                ["page"] = page ?? "1",
            };
            string endpoint = _apiService.BuildEndpoint("movie/popular", queryParams);
            _logger.Log($"Fetching the results for the popular movies query", LogTypes.INFO);
            string data = await _apiService.FetchDataAsync(endpoint);
            return Ok(data);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingMovies([FromQuery] string? page)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>
            {
                ["language"] = "en-US",
                ["page"] = page ?? "1",
            };
            string endpoint = _apiService.BuildEndpoint("movie/upcoming", queryParams);
            _logger.Log($"Fetching the results for the upcoming movies query", LogTypes.INFO);
            string data = await _apiService.FetchDataAsync(endpoint);
            return Ok(data);
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedMovies([FromQuery] string? page)
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>
            {
                ["language"] = "en-US",
                ["page"] = page ?? "1",
            };
            string endpoint = _apiService.BuildEndpoint("movie/top_rated", queryParams);
            _logger.Log($"Fetching the results for the toprated movies query", LogTypes.INFO);
            string data = await _apiService.FetchDataAsync(endpoint);
            return Ok(data);
        }

    }
}
