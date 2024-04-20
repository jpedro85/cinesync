using Azure;
using Microsoft.Extensions.Hosting;

namespace CineSync.Data.Models
{
    public class CollectionsMovies
    {
        public uint Id { get; set; }

        public uint MovieId { get; set; }

        public uint MovieCollectionId { get; set; }

        public Movie Movie { get; set; } = null!;

        public MovieCollection MovieCollection { get; set; } = null!;

    }
}
