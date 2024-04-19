using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger
{
    public interface ILoggerStrategy
    {
        public void Log(string message, LogTypes? type = null);
    }
}
