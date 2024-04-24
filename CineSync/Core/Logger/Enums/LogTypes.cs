namespace CineSync.Core.Logger.Enums
{
    /// <summary>
    /// Defines the types of logs that can be generated.
    /// </summary>
    public enum LogTypes : byte
    {
        /// <summary>
        /// Represents an informational message. This is typically used to output non-critical messages that provide insights into the operation of the application at a high level.
        /// </summary>
        INFO,
        /// <summary>
        /// Represents a debug-level message. This is used for messages that are helpful in diagnosing problems and typically contains more detailed information than INFO level.
        /// </summary>
        DEBUG,
        /// <summary>
        /// Represents a warning message. This indicates a potential issue in the application, but it does not prevent the application from continuing its operation.
        /// </summary>
        WARN,
        /// <summary>
        /// Represents an error message. This indicates a significant problem that has occurred within the application, which might prevent normal operations.
        /// </summary>
        ERROR,
    }
}
