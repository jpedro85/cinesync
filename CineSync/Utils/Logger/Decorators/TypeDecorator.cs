namespace CineSync.Utils.Logger.Decorators
{
    public class TypeDecorator : ILoggerStrategy
    {
        private readonly ILoggerStrategy _wrappedLogger;
        private readonly string _type;

        public TypeDecorator(ILoggerStrategy wrappedLogger, string type)
        {
            _wrappedLogger = wrappedLogger;
            _type = type;
        }

        public void Log(string message)
        {
            _wrappedLogger.Log($"[{_type}] {message}");
        }
    }
}
