namespace CineSync.Utils.Logger.Decorators
{
    public class TimeStampDecorator : ILoggerStrategy
    {
        private ILoggerStrategy _wrappedLogger;

        public TimeStampDecorator(ILoggerStrategy wrappedLogger)
        {
            _wrappedLogger = wrappedLogger;
        }

        public void Log(string message)
        {
            string decoratedMessage = $"[{DateTime.Now}]: {message}";
            _wrappedLogger.Log(decoratedMessage);
        }
    }
}
