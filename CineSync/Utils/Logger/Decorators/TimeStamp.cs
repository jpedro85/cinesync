using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Decorators
{
    public class TimeStamp : LoggerDecorator
    {
        public TimeStamp(ILoggerStrategy wrappedLogger) : base(wrappedLogger) { }

        public override void Log(string message, LogTypes? type)
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
            string decoratedMessage = $"[{formattedDate}]: {message}";
            base.Log(decoratedMessage, type);
        }
    }
}

