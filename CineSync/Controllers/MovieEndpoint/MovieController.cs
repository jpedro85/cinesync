using CineSync.Core.Adapters.ApiAdapters;
using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Core.Logger.Enums;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CineSync.Controllers.MovieEndpoint
{
    /// <summary>
    /// Handles movie-related HTTP requests, including fetching movies by ID, searching, and retrieving lists of popular, upcoming, or top-rated movies.
    /// </summary>
    [Route("movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieManager _movieManager;
        private readonly ApiService _apiService;
        private readonly ILoggerStrategy _logger;
        private readonly MovieDetailsAdapter _movieDetailsAdapter;
        private readonly IUnitOfWorkAsync _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieController"/> class.
        /// </summary>
        /// <param name="apiService">Service for API calls.</param>
        /// <param name="logger">Logger for logging messages.</param>
        /// <param name="movieManager">Manager for database operations related to movies.</param>
        /// <param name="movieDetailsAdapter">Adapter for converting JSON data to movie models.</param>
        public MovieController(ApiService apiService, ILoggerStrategy logger, MovieManager movieManager, MovieDetailsAdapter movieDetailsAdapter)
        {
            _apiService = apiService;
            _logger = logger;
            _movieManager = movieManager;
            _movieDetailsAdapter = movieDetailsAdapter;
        }

        /// <summary>
        /// Retrieves movie details by movie ID.
        /// </summary>
        /// <param name="id">The TMDB movie ID.</param>
        /// <returns>An IActionResult containing the movie details.</returns>
        [HttpGet]
        public async Task<IActionResult> GetMovieById([FromQuery] int id)
        {
            _logger.Log($"Fetching the Movie details {id}", LogTypes.INFO);
            Movie? databaseResult = await _movieManager.GetByTmdbId(id);

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

        /// <summary>
        /// Searches for movies based on a query string and optional filters.
        /// </summary>
        /// <param name="parameters">Search parameters including query string and filters like language and include_adult.</param>
        /// <returns>An IActionResult containing the search results.</returns>
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


        /// <summary>
        /// Retrieves a list of popular movies.
        /// </summary>
        /// <param name="page">The page number of movie results to fetch (optional, default is 1).</param>
        /// <returns>An IActionResult containing a list of popular movies.</returns>
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

        /// <summary>
        /// Retrieves a list of upcoming movies from the API.
        /// </summary>
        /// <param name="page">Optional page number for pagination; defaults to page 1 if not specified.</param>
        /// <returns>An IActionResult containing a list of upcoming movies.</returns>
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

        /// <summary>
        /// Retrieves a list of top-rated movies from the API.
        /// </summary>
        /// <param name="page">Optional page number for pagination; defaults to page 1 if not specified.</param>
        /// <returns>An IActionResult containing a list of top-rated movies, limited to the top 10 entries.</returns>
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
            apiResponse.Results = apiResponse.Results.Take(10).ToList();

            return Ok(apiResponse);
        }

    }
}
