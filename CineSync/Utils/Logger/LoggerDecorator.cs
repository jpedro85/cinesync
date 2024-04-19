using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger
{
    public abstract class LoggerDecorator : ILoggerStrategy
    {

        private readonly ILoggerStrategy _wrappedLogger;

        public LoggerDecorator(ILoggerStrategy logger)
        {
            _wrappedLogger = logger;
        }

        public virtual void Log(string message, LogTypes? type = null)
        {
            _wrappedLogger.Log(message, type);
        }
    }
}
