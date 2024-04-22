using CineSync.Utils.Logger.Enums;
using System.Diagnostics;

namespace CineSync.Utils.Logger.Strategies
{
    /// <summary>
    /// A logging strategy that outputs log messages to the trace listeners configured in the application.
    /// </summary>
    /// <remarks>
    /// Trace logging is useful for detailed output of program execution and state, allowing developers to monitor application behavior in detail.
    /// It can be particularly beneficial in complex systems where understanding the flow of execution and the state changes is crucial.
    /// </remarks>
    public class TraceLogger : ILoggerStrategy
    {
        /// <summary>
        /// Logs a message to the configured trace listeners.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, but this implementation does not utilize the type for any special formatting or processing.</param>
        /// <remarks>
        /// The message is output as-is to all active trace listeners, which can include debug windows, log files, or other diagnostic monitors depending on the application's configuration.
        /// </remarks>
        public void Log(string message, LogTypes? type)
        {
            Trace.WriteLine(message);
        }
    }
}
