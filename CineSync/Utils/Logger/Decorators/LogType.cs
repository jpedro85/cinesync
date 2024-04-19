using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Decorators
{
    /// <summary>
    /// A decorator that prefixes the log message with its log type.
    /// </summary>
    /// <remarks>
    /// This decorator enhances the logging message by prepending a string representation of the log type
    /// (e.g., [INFO], [DEBUG], [WARN], [ERROR]) to the log message. This helps in categorizing and filtering log output.
    /// </remarks>
    public class LogType : LoggerDecorator
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LogType"/> class, wrapping the specified logger.
        /// </summary>
        /// <param name="wrappedLogger">The logger instance to wrap.</param>
        public LogType(ILoggerStrategy wrappedLogger) : base(wrappedLogger) { }

        /// <summary>
        /// Logs a message with an optional type, decorating it by prefixing the log type.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, which is used to prefix the message.</param>
        /// <remarks>
        /// If a log type is provided, the message is prefixed with the log type enclosed in square brackets.
        /// This method calls the <see cref="LoggerDecorator.Log"/> method of the base class to continue the logging process.
        /// </remarks>
        public override void Log(string message, LogTypes? type)
        {
            if (type.HasValue())
            {
                string decoratedMessage = $"[{type.ToString()}] {message}";
                base.Log(decoratedMessage, type);
            }
            else
                base.Log(message, type);
        }

    }
}
