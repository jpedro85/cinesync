namespace CineSync.Utils.Logger
{
    public class CompositeLogger : ILoggerStrategy
    {
        public List<ILoggerStrategy> Loggers { get; } = new List<ILoggerStrategy>();

        public void AddLogger(ILoggerStrategy logger)
        {
            Loggers.Add(logger);
        }

        public void Log(string message)
        {
            foreach (var logger in Loggers)
            {
                logger.Log(message);
            }
        }

    }
}
