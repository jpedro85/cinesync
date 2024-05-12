using CineSync.Data;
using CineSync.Data.Models;
using Newtonsoft.Json.Linq;

namespace CineSync.Core.Adapters.ApiAdapters
{
    /// <summary>
    /// Handles the conversion of JSON movie data from an external API into a Movie object,
    /// and manages the fetching of related assets like posters and trailers.
    /// </summary>
    public class MovieDetailsAdapter
    {
        private static readonly string ImageServiceBaseUri = "https://image.tmdb.org/t/p/w200/";
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the MovieDetailsAdapter class.
        /// </summary>
        /// <param name="dbContext">The application's database context.</param>
        /// <param name="httpClientFactory">A factory for creating HttpClient instances to fetch external resources.</param>
        public MovieDetailsAdapter(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Converts a JSON string into a Movie object, fetching additional details like poster images and genre information.
        /// </summary>
        /// <param name="json">The JSON string containing movie data.</param>
        /// <returns>A fully populated Movie object.</returns>
        public async Task<Movie> FromJson(string json)
        {
            JObject jObject = JObject.Parse(json);
            Movie movie = ParseMovie(jObject);
            await FetchPosterImageAsync(movie, jObject["poster_path"]?.ToString());
            await HandleMovieGenresAsync(movie, jObject["genres"]);
            AssignTrailerKey(movie, jObject["videos"]?["results"]);
            return movie;
        }

        /// <summary>
        /// Parses basic movie details from a JObject.
        /// </summary>
        /// <param name="jObject">The JObject parsed from the JSON string.</param>
        /// <returns>A Movie object with basic properties set.</returns>
        private Movie ParseMovie(JObject jObject)
        {
            return new Movie
            {
                MovieId = (int)jObject["id"]!,
                Title = (string)jObject["title"]!,
                Overview = (string)jObject["overview"]!,
                ReleaseDate = string.IsNullOrEmpty((string?)jObject["release_date"]) ? (DateTime?)null : DateTime.Parse((string)jObject["release_date"]),
                RunTime = (short)jObject["runtime"]!,
                Rating = (float)jObject["vote_average"]!,
                Cast = jObject["credits"]!["cast"]!.Take(10).Select(cast => (string)cast["name"]!).ToList()
            };
        }

        /// <summary>
        /// Asynchronously fetches the movie's poster image from a specified URL.
        /// </summary>
        /// <param name="movie">The movie object where the poster image will be stored.</param>
        /// <param name="posterPath">The partial path to the poster image to fetch.</param>
        private async Task FetchPosterImageAsync(Movie movie, string? posterPath)
        {
            if (!string.IsNullOrEmpty(posterPath))
            {
                HttpClient client = _httpClientFactory.CreateClient();
                movie.PosterImage = await FetchImageAsync(ImageServiceBaseUri + posterPath, client);
            }
        }

        /// <summary>
        /// Handles the association of genres with the movie, ensuring they exist in the database, and linking them.
        /// </summary>
        /// <param name="movie">The movie to which genres will be linked.</param>
        /// <param name="jGenres">The JSON token containing genre data.</param>
        private async Task HandleMovieGenresAsync(Movie movie, JToken? jGenres)
        {
            var genreTasks = jGenres?.Select(jGenre => EnsureGenreExists((int)jGenre["id"]!, (string)jGenre["name"]!)).ToList();
            if (genreTasks != null)
            {
                movie.Genres = await Task.WhenAll(genreTasks);
            }
        }

        /// <summary>
        /// Fetches an image from a specified URL and returns it as a byte array.
        /// </summary>
        /// <param name="url">The URL of the image to fetch.</param>
        /// <param name="client">The HttpClient instance to use for the fetch operation.</param>
        /// <returns>A byte array containing the fetched image.</returns>
        private async Task<byte[]> FetchImageAsync(string url, HttpClient client)
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            throw new Exception("Failed to download the image.");
        }

        /// <summary>
        /// Ensures a genre exists in the database, adding it if it does not.
        /// </summary>
        /// <param name="tmdbId">The TMDB ID of the genre.</param>
        /// <param name="name">The name of the genre.</param>
        /// <returns>The Genre object, either newly created or retrieved from the database.</returns>
        private async Task<Genre> EnsureGenreExists(int tmdbId, string name)
        {
            var existingGenre = _dbContext.Set<Genre>().FirstOrDefault(g => g.TmdbId == tmdbId);
            if (existingGenre == null)
            {
                var newGenre = new Genre { TmdbId = tmdbId, Name = name };
                _dbContext.Set<Genre>().Add(newGenre);
                await _dbContext.SaveChangesAsync();
                return newGenre;
            }
            return existingGenre;
        }

        /// <summary>
        /// Assigns the trailer key to the movie object if a trailer is available in the video results.
        /// </summary>
        /// <param name="movie">The movie object where the trailer key will be stored.</param>
        /// <param name="videoResults">The JSON token containing video data.</param>
        private void AssignTrailerKey(Movie movie, JToken? videoResults)
        {
            if (videoResults == null) return;

            // Look for the first video marked as a "Trailer"
            var trailer = videoResults.FirstOrDefault(video => (string)video["type"]! == "Trailer" && video["key"] != null);

            if (trailer != null)
            {
                movie.TrailerKey = (string)trailer["key"]!;
            }
        }
    }
}
