using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Utils.Logger;
using CineSync.Utils.Logger.Enums;
using CineSync.Utils.Adapters.ApiAdapters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CineSync.Controllers.MovieEndpoint
{

    [Route("movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieManager _movieManager;
        private readonly ApiService _apiService;
        private readonly ILoggerStrategy _logger;
        private readonly MovieDetailsAdapter _movieDetailsAdapter;

        public MovieController(ApiService apiService, ILoggerStrategy logger, MovieManager movieManager, MovieDetailsAdapter movieDetailsAdapter)
        {
            _apiService = apiService;
            _logger = logger;
            _movieManager = movieManager;
            _movieDetailsAdapter = movieDetailsAdapter;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovieById([FromQuery] int id)
        {
            _logger.Log($"Fetching the Movie details {id}", LogTypes.INFO);
            var databaseResult = await _movieManager.GetByTmdbId(id);

            if (databaseResult != null)
            {
                _logger.Log($"Fetched the Movie details for {id} from the database, Successfully", LogTypes.DEBUG);
                return Ok(databaseResult);
            }

            // In case its not on the database
            string endpoint = $"movie/{id}?append_to_response=credits,videos";
            string data = await _apiService.FetchDataAsync(endpoint);
            Movie movie = await _movieDetailsAdapter.FromJson(data);
            // Add to the Database async
            _movieManager.AddAsync(movie);
            _logger.Log($"Fetched the Movie details for {id} from the API, Successfully", LogTypes.INFO);

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

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new MovieConverter());
            ApiSearchResponse apiResponse = JsonConvert.DeserializeObject<ApiSearchResponse>(data, settings)!;
            _logger.Log($"Fetched the query results for {parameters.Query}, successfully", LogTypes.INFO);

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

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new MovieConverter());
            ApiSearchResponse apiResponse = JsonConvert.DeserializeObject<ApiSearchResponse>(data, settings)!;
            _logger.Log($"Fetched the results for the popular movies query successfully", LogTypes.INFO);

            return Ok(apiResponse);
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

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new MovieConverter());
            ApiSearchResponse apiResponse = JsonConvert.DeserializeObject<ApiSearchResponse>(data, settings)!;
            _logger.Log($"Fetched the results for the upcoming movies query successfully", LogTypes.INFO);

            return Ok(apiResponse);
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

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new MovieConverter());
            ApiSearchResponse apiResponse = JsonConvert.DeserializeObject<ApiSearchResponse>(data, settings)!;
            _logger.Log($"Fetched the results for the top-rated movies query successfully", LogTypes.INFO);

            return Ok(apiResponse);
        }

    }
}
