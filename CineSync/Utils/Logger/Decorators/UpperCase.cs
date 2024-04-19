using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Decorators
{
    public class UpperCase : LoggerDecorator
    {
        public UpperCase(ILoggerStrategy wrappedLogger) : base(wrappedLogger) { }

        public override void Log(string message, LogTypes? type)
        {
            string decoratedMessage = message.ToUpper();
            base.Log(decoratedMessage, type);
        }
    }
}
