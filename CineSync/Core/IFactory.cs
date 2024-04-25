namespace CineSync.Core
{
    public interface IFactory
    {
        T? Create<T>(params object?[]? args);
    }
}
