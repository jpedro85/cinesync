namespace CineSync.Utils.Adapters
{
    public interface IMovie
    {
        int MovieId { get; }
        byte[] PosterImage { get; }
        ICollection<string> Genres { get; }
        string Overview { get; }
        DateTime ReleaseDate { get; }
        ICollection<string> Cast { get; }
        string TrailerKey { get; }
        short RunTime { get; }
        Half Rating { get; }
    }
}
