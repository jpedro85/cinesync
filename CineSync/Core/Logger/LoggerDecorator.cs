using CineSync.Core.Logger.Enums;

namespace CineSync.Core.Logger
{
    /// <summary>
    /// Provides a base for decorator implementations in the logging system, allowing for additional behaviors to be added to loggers dynamically.
    /// </summary>
    /// <remarks>
    /// Decorators derived from this class can modify or extend the behavior of the wrapped logger instance provided during construction.
    /// </remarks>
    public abstract class LoggerDecorator : ILoggerStrategy
    {

        private readonly ILoggerStrategy _wrappedLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerDecorator"/> class, wrapping the specified logger.
        /// </summary>
        /// <param name="logger">The logger instance to wrap. This instance will be used to perform the actual logging, with potentially modified behavior by this decorator.</param>
        public LoggerDecorator(ILoggerStrategy logger)
        {
            _wrappedLogger = logger;
        }

        /// <summary>
        /// Logs a message with an optional type, delegating the logging to the wrapped logger instance.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, which may influence how the message is formatted or processed by the wrapped logger.</param>
        /// <remarks>
        /// This method calls the <see cref="Log"/> method on the wrapped logger. Subclasses should override this method to add custom behavior before or after the log operation is delegated.
        /// </remarks>
        public virtual void Log(string message, LogTypes? type = null)
        {
            _wrappedLogger.Log(message, type);
        }
    }
}
