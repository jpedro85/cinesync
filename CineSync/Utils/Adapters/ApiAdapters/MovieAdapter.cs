using Newtonsoft.Json;
using System.Text;

namespace CineSync.Utils.Adapters.ApiAdapters
{
    public class MovieAdapter
    {
        public int MovieId { get; private set; }
        public byte[] PosterImage { get; private set; }
        public ICollection<string> Genres { get; private set; }
        public string Overview { get; private set; }
        public DateTime ReleaseDate { get; private set; }
        public ICollection<string> Cast { get; private set; }
        public string TrailerKey { get; private set; }
        public short RunTime { get; private set; }
        public Half Rating { get; private set; }

        public async static Task<MovieAdapter> FromJson(string jsonString)
        {
            var rawResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            MovieAdapter adapter = new MovieAdapter
            {
                MovieId = rawResponse.id,
                Overview = rawResponse.overview,
                ReleaseDate = DateTime.Parse((string)rawResponse.release_date),
                RunTime = (short)rawResponse.runtime,
                Genres = ((IEnumerable<dynamic>)rawResponse.genres).Select(genre => (string)genre.name).ToList(),
                Cast = ((IEnumerable<dynamic>)rawResponse.credits.cast).Select(cast => (string)cast.name).ToList(),
                TrailerKey = ((IEnumerable<dynamic>)rawResponse.videos.results).First(video => video.type == "Trailer").key,
                Rating = (Half)(float)rawResponse.vote_average
            };

            string endpoint = rawResponse.poster_path;
            adapter.PosterImage = await FetchImageAsync(endpoint);

            return adapter;
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
            sb.AppendLine($"Image: {PosterImage}");

            return sb.ToString();
        }

        private static async Task<byte[]> FetchImageAsync(string endpoint)
        {
            string imageService = "https://image.tmdb.org/t/p/w200/";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(imageService + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                throw new Exception("Failed to download the image.");
            }
        }
    }
}
