using CineSync.Data;
using CineSync.Data.Models;
using Newtonsoft.Json.Linq;

namespace CineSync.Core.Adapters.ApiAdapters
{
    public class MovieDetailsAdapter
    {
        private static readonly string _imageService = "https://image.tmdb.org/t/p/w200/";
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpClient _client = new HttpClient();

        public MovieDetailsAdapter(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Movie> FromJson(string json)
        {
            JObject jObject = JObject.Parse(json);

            Movie movie = new Movie
            {
                MovieId = (int)jObject["id"]!,
                Title = (string)jObject["title"]!,
                Overview = (string)jObject["overview"]!,
                ReleaseDate = DateTime.Parse((string)jObject["release_date"]!),
                RunTime = (short)jObject["runtime"]!,
                Rating = (float)jObject["vote_average"]!,
                Cast = jObject["credits"]!["cast"]!.Take(10).Select(cast => (string)cast["name"]!).ToList(),
            };

            // Fetches the Images asynchronously while its either saving the Genres or fetching them
            string posterPath = (string)jObject["poster_path"]!;
            if (!string.IsNullOrEmpty(posterPath))
            {
                movie.PosterImage = await FetchImageAsync(_imageService + posterPath);
            }

            JToken? videoResults = jObject["videos"]?["results"];
            if (videoResults != null)
            {
                movie.TrailerKey = videoResults?.FirstOrDefault(video => (string)video["type"] == "Trailer" && video["key"] != null)?["key"]?.ToString();
            }

            List<Task<Genre>> genreTasks = new List<Task<Genre>>();
            JToken? jGenres = jObject["genres"];
            if (jGenres != null)
            {
                foreach (var jGenre in jObject["genres"]!)
                {
                    genreTasks.Add(EnsureGenreExists((int)jGenre["id"]!, (string)jGenre["name"]!));
                }
            }

            movie.Genres = await Task.WhenAll(genreTasks);

            return movie;
        }

        private async Task<byte[]> FetchImageAsync(string url)
        {
            var response = await _client.GetAsync(url);
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

    }
}
