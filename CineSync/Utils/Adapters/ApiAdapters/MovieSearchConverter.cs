using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CineSync.Utils.Adapters.ApiAdapters
{

    public class MovieConverter : JsonConverter
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string imageService = "https://image.tmdb.org/t/p/w200/";

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(MovieSearchAdapter));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            var movie = new MovieSearchAdapter
            {
                Id = (int)obj["id"],
                Title = (string)obj["title"],
                PosterPath = (string)obj["poster_path"]
            };

            if (!string.IsNullOrWhiteSpace(movie.PosterPath))
            {
                string fullPath = imageService + movie.PosterPath;
                var response = client.GetAsync(fullPath).Result; // Use Result for synchronous wait within async context
                if (response.IsSuccessStatusCode)
                {
                    movie.PosterImage = response.Content.ReadAsByteArrayAsync().Result;
                }
            }

            return movie;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        public override bool CanWrite => false;
    }
}
