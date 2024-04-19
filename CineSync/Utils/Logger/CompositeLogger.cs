using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger
{
    /// <summary>
    /// Represents a logger that maintains a collection of different logging strategies (loggers),
    /// allowing for simultaneous logging through multiple strategies.
    /// </summary>
    public class CompositeLogger : ILoggerStrategy
    {
        /// <summary>
        /// Gets the list of loggers included in this composite logger.
        /// </summary>
        public List<ILoggerStrategy> Loggers { get; } = new List<ILoggerStrategy>();

        /// <summary>
        /// Adds a logger to the composite logger.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        /// <remarks>
        /// Each logger added will receive log messages when the <see cref="Log"/> method is called.
        /// </remarks>
        public void AddLogger(ILoggerStrategy logger)
        {
            Loggers.Add(logger);
        }

        /// <summary>
        /// Logs a message through all loggers in the composite logger.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="type">Optional. The type of the log message, based on the <see cref="LogTypes"/> enum.</param>
        /// <remarks>
        /// This method will pass the message to each logger in the collection, optionally prefixing it with the specified log type.
        /// Each logger handles the message based on its configuration (e.g., to console, file, etc.).
        /// Also the type will only work on the Loggers that have the LogType Decorator implemented else it will do nothing.
        /// </remarks>
        public void Log(string message, LogTypes? type)
        {
            foreach (var logger in Loggers)
            {
                logger.Log(message, type);
            }
        }

    }
}
