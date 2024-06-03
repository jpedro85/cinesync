using System.Text.Json.Serialization;

namespace CineSync.Data.Models
{

    public class UserImage
    {
        public uint Id { get; set; }

        public string UserId { get; set; }

        public byte[] ImageData { get; set; }

        public string ContentType { get; set; }

        [JsonIgnore]
        public ApplicationUser User { get; set; }
    }
}
