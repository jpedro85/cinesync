using System.Diagnostics;
using CineSync.Core.Logger.Enums;

namespace CineSync.Core.Logger.Strategies
{
    /// <summary>
    /// A logging strategy that outputs log messages to the debug console.
    /// </summary>
    /// <remarks>
    /// This logger is particularly useful for development purposes where messages need to be examined without impacting the application's runtime performance or user experience. It allows developers to monitor internal application states without displaying these logs to end users.
    /// </remarks>
    public class DebugLogger : ILoggerStrategy
    {
        /// <summary>
        /// Logs a message to the debug output window of the development environment.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, but this implementation does not utilize the type for any special formatting or processing.</param>
        /// <remarks>
        /// The message is output as-is to the debug console. This method provides a simple way to track application behavior during development and is not intended for use in a production environment.
        /// </remarks>
        public void Log(string message, LogTypes? type)
        {
            Debug.WriteLine(message);
        }
    }
}
