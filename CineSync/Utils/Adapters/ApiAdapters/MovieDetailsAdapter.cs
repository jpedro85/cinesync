using CineSync.Data.Models;
using Newtonsoft.Json.Linq;
using System.Text;

namespace CineSync.Utils.Adapters.ApiAdapters
{
    public class MovieDetailsAdapter : IMovie
    {
        private readonly static string _imageService = "https://image.tmdb.org/t/p/w200/";
        public int MovieId { get; set; }
        public string Title { get; set; }
        public byte[] PosterImage { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public string Overview { get; set; }
        public DateTime ReleaseDate { get; set; }
        public ICollection<string> Cast { get; set; }
        public string TrailerKey { get; set; }
        public short RunTime { get; set; }
        public float Rating { get; set; }

        public static MovieDetailsAdapter FromJson(string json)
        {
            var jObject = JObject.Parse(json);

            var adapter = new MovieDetailsAdapter
            {
                MovieId = (int)jObject["id"],
                Title = (string)jObject["title"],
                Overview = (string)jObject["overview"],
                ReleaseDate = DateTime.Parse((string)jObject["release_date"]),
                RunTime = (short)jObject["runtime"],
                Rating = (float)jObject["vote_average"],
                Genres = jObject["genres"].Select(j => new Genre { TmdbId = (int)j["id"], Name = (string)j["name"] }).ToList(),
                Cast = jObject["credits"]["cast"].Take(10).Select(castObj => (string)castObj["name"]).ToList(),
                TrailerKey = jObject["videos"]["results"].FirstOrDefault(videoObj => (bool)videoObj["official"] && (string)videoObj["site"] == "YouTube")?["key"].ToString()
            };

            string posterPath = (string)jObject["poster_path"];
            if (!string.IsNullOrEmpty(posterPath))
            {
                adapter.PosterImage = FetchImageAsync(_imageService + posterPath).Result;
            }

            return adapter;
        }

        private static async Task<byte[]> FetchImageAsync(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                throw new Exception("Failed to download the image.");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Movie ID: {MovieId}");
            sb.AppendLine($"Overview: {Overview}");
            sb.AppendLine($"Release Date: {ReleaseDate.ToShortDateString()}");
            sb.AppendLine($"Run Time: {RunTime} minutes");
            sb.AppendLine($"Genres: {string.Join(", ", Genres)}");
            sb.AppendLine($"Cast: {string.Join(", ", Cast)}");
            sb.AppendLine($"Trailer Key: {TrailerKey}");
            sb.AppendLine($"Rating: {Rating}");

            return sb.ToString();
        }
    }
}
