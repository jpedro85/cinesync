using CineSync.Utils.Logger;
using CineSync.Utils.Logger.Enums;
using System.Net.Http.Headers;
using System.Web;

namespace CineSync.Controllers
{
    /// <summary>
    /// Provides a service for making HTTP requests to The Movie Database (TMDb) API.
    /// </summary>
    /// <remarks>
    /// This service is pre-configured with authorization and default headers necessary for making requests to TMDb.
    /// </remarks>
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly string? BEARER_TOKEN = Environment.GetEnvironmentVariable("BEARER_TOKEN");
        private readonly ILoggerStrategy _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiService"/> class, configuring it for communication with the TMDb API.
        /// </summary>
        /// <param name="client">The HttpClient instance to be used for making requests.</param>
        public ApiService(HttpClient client, ILoggerStrategy logger)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BEARER_TOKEN);
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously fetches data from a specified endpoint of the TMDb API.
        /// </summary>
        /// <param name="endpoint">The API endpoint from which data is to be fetched, appended to the base URI.</param>
        /// <returns>A string containing the JSON response from the API or null if an error occurs.</returns>
        /// <remarks>
        /// This method attempts to retrieve data from the specified API endpoint. If the request fails due to an HTTP error, it logs the error to the console and returns null.
        /// </remarks>
        public async Task<string?> FetchDataAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                _logger.Log($"Message: {e.Message}", LogTypes.ERROR);
                throw;
            }
        }

        public string BuildEndpoint(string baseEndpoint, Dictionary<string, string> queryParams)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }
            return $"{baseEndpoint}?{query.ToString()}";
        }
    }
}
