using CineSync.Data;
using CineSync.Data.Models;
using Newtonsoft.Json.Linq;

namespace CineSync.Core.Adapters.ApiAdapters
{
    public class MovieDetailsAdapter
    {
        private static readonly string ImageServiceBaseUri = "https://image.tmdb.org/t/p/w200/";
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieDetailsAdapter(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Movie> FromJson(string json)
        {
            JObject jObject = JObject.Parse(json);
            Movie movie = ParseMovie(jObject);
            await FetchPosterImageAsync(movie, jObject["poster_path"]?.ToString());
            await HandleMovieGenresAsync(movie, jObject["genres"]);
            AssignTrailerKey(movie, jObject["videos"]?["results"]);
            return movie;
        }
        
        private Movie ParseMovie(JObject jObject)
        {
            return new Movie
            {
                MovieId = (int)jObject["id"]!,
                Title = (string)jObject["title"]!,
                Overview = (string)jObject["overview"]!,
                ReleaseDate = DateTime.Parse((string)jObject["release_date"]!),
                RunTime = (short)jObject["runtime"]!,
                Rating = (float)jObject["vote_average"]!,
                Cast = jObject["credits"]!["cast"]!.Take(10).Select(cast => (string)cast["name"]!).ToList()
            };
        } 
        
        private async Task FetchPosterImageAsync(Movie movie, string? posterPath)
        {
            if (!string.IsNullOrEmpty(posterPath))
            {
                HttpClient client = _httpClientFactory.CreateClient();
                movie.PosterImage = await FetchImageAsync(ImageServiceBaseUri + posterPath, client);
            }
        } 
        
        private async Task HandleMovieGenresAsync(Movie movie, JToken? jGenres)
        {
            var genreTasks = jGenres?.Select(jGenre => EnsureGenreExists((int)jGenre["id"]!, (string)jGenre["name"]!)).ToList();
            if (genreTasks != null)
            {
                movie.Genres = await Task.WhenAll(genreTasks);
            }
        } 

        private async Task<byte[]> FetchImageAsync(string url, HttpClient client)
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            throw new Exception("Failed to download the image.");
        }

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
