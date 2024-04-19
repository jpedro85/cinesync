using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Decorators
{
    public class LogType : LoggerDecorator
    {

        public LogType(ILoggerStrategy wrappedLogger) : base(wrappedLogger) { }

        public override void Log(string message, LogTypes? type)
        {
            string decoratedMessage = $"[{type.ToString()}] {message}";
            base.Log(decoratedMessage, type);
        }

    }
}
