using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace CineSync.Controllers.Movie
{
    // TODO: Add the functionality to search first on our DB
    [Route("movie")]
    [ApiController]
    public class MovieService : ControllerBase
    {
        private readonly ApiService _apiService;

        public MovieService(ApiService apiService)
        {
            _apiService = apiService;
        }

        private string BuildEndpoint(string baseEndpoint, Dictionary<string, string> queryParams)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }
            return $"{baseEndpoint}?{query.ToString()}";
        }

        private async Task<IActionResult> ProcessRequestAsync(string endpoint)
        {
            try
            {
                string data = await _apiService.FetchDataAsync(endpoint);
                return Ok(data);
            }
            catch (Exception ex)
            {
                // NOTE: Needs Logger
                return StatusCode(500, "An error occurred while processing your request.");
            }
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

            try
            {
                return await ProcessRequestAsync(endpoint);
            }
            catch (Exception ex)
            {
                // NOTE: Needs Logger
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies([FromQuery] string? page)
        {
            try
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>
                {
                    ["language"] = "en-US",
                    ["page"] = page ?? "1",
                };
                string endpoint = BuildEndpoint("movie/popular", queryParams);
                return await ProcessRequestAsync(endpoint);
            }
            catch (Exception ex)
            {
                // NOTE: Needs Logger
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingMovies([FromQuery] string? page)
        {
            try
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>
                {
                    ["language"] = "en-US",
                    ["page"] = page ?? "1",
                };
                string endpoint = BuildEndpoint("movie/upcoming", queryParams);
                return await ProcessRequestAsync(endpoint);
            }
            catch (Exception ex)
            {
                // NOTE: Needs Logger
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedMovies([FromQuery] string? page)
        {
            try
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>
                {
                    ["language"] = "en-US",
                    ["page"] = page ?? "1",
                };
                string endpoint = BuildEndpoint("movie/top_rated", queryParams);
                return await ProcessRequestAsync(endpoint);
            }
            catch (Exception ex)
            {
                // NOTE: Needs Logger
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
