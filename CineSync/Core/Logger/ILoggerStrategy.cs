using CineSync.Core.Logger.Enums;

namespace CineSync.Core.Logger
{
    /// <summary>
    /// Defines a contract for logging strategies used within the logging system.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are expected to handle the logging of messages according to specific strategies (e.g., to console, files, etc.).
    /// </remarks>
    public interface ILoggerStrategy
    {
        /// <summary>
        /// Logs a message with an optional type.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, which can influence how the message is formatted or processed.</param>
        /// <remarks>
        /// The method should implement the logic necessary to record the message to the chosen log medium (console, file, etc.).
        /// If a type is provided, it can be used to prefix the message or otherwise alter its handling to reflect its importance or category (INFO, ERROR, etc.),
        /// only works if the logger has the LogType decorator implemented
        /// </remarks>
        public void Log(string message, LogTypes? type = null);
    }
}
