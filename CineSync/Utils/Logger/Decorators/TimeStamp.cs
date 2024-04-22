using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Decorators
{
    /// <summary>
    /// A decorator that prefixes the log message with the current date and time.
    /// </summary>
    /// <remarks>
    /// This decorator enhances the logging output by adding a timestamp before the log message.
    /// The format of the timestamp is "yyyy-MM-dd_HH:mm:ss", which provides a precise indication of when the log entry was made.
    /// </remarks>
    public class TimeStamp : LoggerDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStamp"/> class, wrapping the specified logger.
        /// </summary>
        /// <param name="wrappedLogger">The logger instance to wrap.</param>
        public TimeStamp(ILoggerStrategy wrappedLogger) : base(wrappedLogger) { }

        /// <summary>
        /// Logs a message with an optional type, decorating it by prefixing the current timestamp.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, which may influence further processing or formatting.</param>
        /// <remarks>
        /// The timestamp is added at the beginning of the log message, followed by a colon and the original message.
        /// This method calls the <see cref="LoggerDecorator.Log"/> method of the base class to continue the logging process.
        /// </remarks>
        public override void Log(string message, LogTypes? type)
        {
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
            string decoratedMessage = $"[{ColorSchemes.YELLOW}{formattedDate}{ColorSchemes.RESET}] {message}";
            base.Log(decoratedMessage, type);
        }
    }
}

