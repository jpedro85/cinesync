using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger
{
    public class CompositeLogger : ILoggerStrategy
    {
        public List<ILoggerStrategy> Loggers { get; } = new List<ILoggerStrategy>();

        public void AddLogger(ILoggerStrategy logger)
        {
            Loggers.Add(logger);
        }

        public void Log(string message, LogTypes? type)
        {
            foreach (var logger in Loggers)
            {
                logger.Log(message, type);
            }
        }

    }
}
