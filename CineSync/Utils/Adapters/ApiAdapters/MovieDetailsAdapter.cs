using CineSync.Data.Models;
using Newtonsoft.Json;
using System.Text;

namespace CineSync.Utils.Adapters.ApiAdapters
{
    public class MovieDetailsAdapter : IMovie
    {
        private readonly dynamic _rawResponse;

        private MovieDetailsAdapter(dynamic rawResponse)
        {
            this._rawResponse = rawResponse;
        }

        public int MovieId => _rawResponse.id;
        public string Title => (string)_rawResponse.title;
        public byte[] PosterImage { get; private set; }
        public ICollection<Genre> Genres => ((IEnumerable<dynamic>)_rawResponse.genres)
                                                .Select(genre => new Genre() { Name = (string)genre.name, TmdbId = (int)genre.Id }).ToList();
        public string Overview => _rawResponse.overview;
        public DateTime ReleaseDate => DateTime.Parse((string)_rawResponse.release_date);
        public ICollection<string> Cast => ((IEnumerable<dynamic>)_rawResponse.credits.cast).Select(cast => (string)cast.name).ToList();
        public string TrailerKey => ((IEnumerable<dynamic>)_rawResponse.videos.results).First(video => video.type == "Trailer").key;
        public short RunTime => (short)_rawResponse.runtime;
        public float Rating => (float)_rawResponse.vote_average;


        public static async Task<IMovie> FromJson(dynamic jsonObject)
        {
            // var rawResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);
            var adapter = new MovieDetailsAdapter(jsonObject);
            await adapter.InitializeAsync();
            return adapter;
        }

        private async Task InitializeAsync()
        {
            Console.WriteLine((string)_rawResponse.poster_path);
            string endpoint = (string) _rawResponse.poster_path;
            if (endpoint != null)
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
