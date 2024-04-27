using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CineSync.Core.Adapters.ApiAdapters
{
    /// <summary>
    /// Provides a custom JSON converter for MovieSearchAdapter objects that includes fetching and caching poster images.
    /// </summary>
    public class MovieConverter : JsonConverter
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string imageService = "https://image.tmdb.org/t/p/w300/";
        private static ConcurrentDictionary<string, byte[]> imageCache = new ConcurrentDictionary<string, byte[]>();

        /// <summary>
        /// Determines whether the current type can be converted by this converter.
        /// </summary>
        /// <param name="objectType">The type of the object to check for convertibility.</param>
        /// <returns>True if the type is MovieSearchAdapter; otherwise, false.</returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(MovieSearchAdapter));
        }

        /// <summary>
        /// Reads JSON data, converting it into a MovieSearchAdapter object and fetching the movie poster image if not already cached.
        /// </summary>
        /// <param name="reader">The JsonReader to read from.</param>
        /// <param name="objectType">The type of the object expected.</param>
        /// <param name="existingValue">The existing value of the object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>A new MovieSearchAdapter object populated with data from the JSON reader.</returns>
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

        /// <summary>
        /// Writes the JSON representation of the object. This method is not implemented as CanWrite returns false.
        /// </summary>
        /// <param name="writer">The JsonWriter to write to.</param>
        /// <param name="value">The object to write.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="NotImplementedException">Thrown to indicate that this method should not be used.</exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        /// <summary>
        /// Gets a value indicating whether this JsonConverter can write JSON.
        /// </summary>
        /// <value>Always false since the converter does not support writing.</value>
        public override bool CanWrite => false;
    }
}
