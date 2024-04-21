using CineSync.Utils.Logger;
using CineSync.Utils.Logger.Enums;
using CineSync.Utils.Adapters.ApiAdapters;
using CineSync.Utils.Adapters;
using CineSync.DbManagers;
using CineSync.Data.Models;
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

        public MovieController(ApiService apiService, ILoggerStrategy logger, MovieManager movieManager)
        {
            _apiService = apiService;
            _logger = logger;
            _movieManager = movieManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovieById([FromQuery] int id)
        {
            var databaseResult = _movieManager.GetByTmdbId(id);

            if (databaseResult != null)
            {
                return Ok(databaseResult);
            }

            // In case its not on the database
            string endpoint = $"movie/{id}?append_to_response=credits,videos";
            _logger.Log($"Fetching the Movie details {id}", LogTypes.INFO);
            string data = await _apiService.FetchDataAsync(endpoint);
            IMovie movie = MovieDetailsAdapter.FromJson(data);
            var databaseMovie = new Movie()
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                PosterImage = movie.PosterImage,
                Genres = movie.Genres,
                Overview = movie.Overview,
                ReleaseDate = movie.ReleaseDate,
                Cast = (IList<string>)movie.Cast,
                TrailerKey = movie.TrailerKey,
                RunTime = movie.RunTime,
                Rating = movie.Rating,
            };
            // Add to the Database async
            _movieManager.AddAsync(databaseMovie);

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

            ApiSearchResponse apiResponse = JsonConvert.DeserializeObject<ApiSearchResponse>(data, settings);

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
