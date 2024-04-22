using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Strategies
{
    /// <summary>
    /// A logging strategy that outputs log messages directly to the console.
    /// </summary>
    /// <remarks>
    /// This logger is ideal for simple applications, debugging sessions, or any scenario where immediate visual feedback is necessary.
    /// </remarks>
    public class ConsoleLogger : ILoggerStrategy
    {
        /// <summary>
        /// Logs a message to the console.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, although this implementation does not utilize the type for any special formatting or processing.</param>
        /// <remarks>
        /// The message is output as-is to the console. If specific formatting based on the log type is required, consider extending this class or using a decorator.
        /// </remarks>
        public void Log(string message, LogTypes? type)
        {
            Console.WriteLine(message);
        }
    }
}
