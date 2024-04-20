using Newtonsoft.Json;
using System.Text;

namespace CineSync.Utils.Adapters.ApiAdapters
{
    public class MovieAdapter : IMovie
    {
        private dynamic rawResponse;

        private MovieAdapter(dynamic rawResponse)
        {
            this.rawResponse = rawResponse;
        }

        public int MovieId => rawResponse.id;
        public byte[] PosterImage { get; private set; }
        public ICollection<string> Genres => ((IEnumerable<dynamic>)rawResponse.genres).Select(genre => (string)genre.name).ToList();
        public string Overview => rawResponse.overview;
        public DateTime ReleaseDate => DateTime.Parse((string)rawResponse.release_date);
        public ICollection<string> Cast => ((IEnumerable<dynamic>)rawResponse.credits.cast).Select(cast => (string)cast.name).ToList();
        public string TrailerKey => ((IEnumerable<dynamic>)rawResponse.videos.results).First(video => video.type == "Trailer").key;
        public short RunTime => (short)rawResponse.runtime;
        public Half Rating => (Half)(float)rawResponse.vote_average;


        public static async Task<IMovie> FromJson(string jsonString)
        {
            var rawResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);
            var adapter = new MovieAdapter(rawResponse);
            await adapter.InitializeAsync();
            return adapter;
        }

        private async Task InitializeAsync()
        {
            string endpoint = rawResponse.poster_path;
            PosterImage = await FetchImageAsync(endpoint);
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
