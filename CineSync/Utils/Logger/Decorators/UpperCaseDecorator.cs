namespace CineSync.Utils.Logger.Decorators
{
    public class UpperCaseDecorator : ILoggerStrategy
    {
        private ILoggerStrategy _wrappedLogger;

        public UpperCaseDecorator(ILoggerStrategy wrappedLogger)
        {
            _wrappedLogger = wrappedLogger;
        }

        public void Log(string message)
        {
            string decoratedMessage = message.ToUpper();
            _wrappedLogger.Log(decoratedMessage);
        }
    }
}
