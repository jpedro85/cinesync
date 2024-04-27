using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CineSync.Core.Adapters.ApiAdapters
{

    public class MovieConverter : JsonConverter
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string imageService = "https://image.tmdb.org/t/p/w300/";
        private static ConcurrentDictionary<string, byte[]> imageCache = new ConcurrentDictionary<string, byte[]>();

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(MovieSearchAdapter));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            MovieSearchAdapter movie = new MovieSearchAdapter
            {
                MovieId = (int)obj["id"]!,
                Title = (string)obj["title"]!,
                PosterPath = (string?)obj["poster_path"]
            };

            if (string.IsNullOrWhiteSpace(movie.PosterPath)) return movie;
            
            string fullPath = imageService + movie.PosterPath;
            byte[] imageBytes;
            // Checks if the path was already fetched on the cache if it is
            // it will give its value to the imageBytes
            if (!imageCache.TryGetValue(fullPath, out imageBytes))
            {
                HttpResponseMessage response = client.GetAsync(fullPath).Result;
                if (response.IsSuccessStatusCode)
                {
                    imageBytes = response.Content.ReadAsByteArrayAsync().Result;
                    imageCache.TryAdd(fullPath, imageBytes);
                }
            }
            movie.PosterImage = imageBytes;

            return movie;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        public override bool CanWrite => false;
    }
}
