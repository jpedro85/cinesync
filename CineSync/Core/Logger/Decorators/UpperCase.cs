using CineSync.Core.Logger.Enums;

namespace CineSync.Core.Logger.Decorators
{
    /// <summary>
    /// A decorator that converts all log messages to uppercase.
    /// </summary>
    /// <remarks>
    /// This decorator is useful for emphasizing log messages or ensuring a consistent case format across various logging outputs.
    /// </remarks>
    public class UpperCase : LoggerDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpperCase"/> class, wrapping the specified logger.
        /// </summary>
        /// <param name="wrappedLogger">The logger instance to wrap.</param>
        public UpperCase(ILoggerStrategy wrappedLogger) : base(wrappedLogger) { }

        /// <summary>
        /// Logs a message with an optional type, transforming it to uppercase before logging.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, which may influence further processing or formatting by other decorators or the final logger.</param>
        /// <remarks>
        /// The transformation to uppercase is applied to the entire message, ensuring that all characters in the log entry are capitalized.
        /// This method calls the <see cref="LoggerDecorator.Log"/> method of the base class to continue the logging process after the transformation.
        /// </remarks>
        public override void Log(string message, LogTypes? type)
        {
            string decoratedMessage = message.ToUpper();
            base.Log(decoratedMessage, type);
        }
    }
}
