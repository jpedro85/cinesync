using CineSync.Data.Models;

namespace CineSync.Utils.Adapters
{
    public interface IMovie
    {
        int MovieId { get; }
        string Title { get; }
        byte[] PosterImage { get; }
        ICollection<Genre> Genres { get; }
        string Overview { get; }
        DateTime ReleaseDate { get; }
        ICollection<string> Cast { get; }
        string TrailerKey { get; }
        short RunTime { get; }
        float Rating { get; }
    }
}
