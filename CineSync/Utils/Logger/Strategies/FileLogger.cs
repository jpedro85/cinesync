using CineSync.Utils.Logger.Enums;
using System.Collections.Concurrent;

namespace CineSync.Utils.Logger.Strategies
{
    /// <summary>
    /// A logging strategy that writes log messages to a specified file.
    /// </summary>
    /// <remarks>
    /// This logger is ideal for scenarios where logs need to be reviewed later or maintained as records.
    /// </remarks>
    public class FileLogger : ILoggerStrategy
    {
        private readonly string _filePath;
        private BlockingCollection<string> _logQueue = new BlockingCollection<string>();
        private static readonly object _lock = new object();
        private Task _loggingTask;
        private bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class, setting the file path where logs will be written.
        /// </summary>
        /// <param name="filePath">The path to the log file.</param>
        public FileLogger(string filePath)
        {
            _filePath = filePath;
            _loggingTask = Task.Run(() => ProcessLogQueue());
        }

        /// <summary>
        /// Logs a message to the specified file.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">Optional. Specifies the type of log entry, but this implementation does not utilize the type for any special formatting or processing.</param>
        /// <remarks>
        /// The message is appended to the file along with a newline. This method uses <see cref="File.AppendAllText"/> which ensures that each message is added to the end of the file.
        /// </remarks>
        public void Log(string message, LogTypes? type)
        {
            _logQueue.Add(message + Environment.NewLine);
            // lock (_lock)
            // {
            //     File.AppendAllText(_filePath, message + Environment.NewLine);
            // }
        }

        private void ProcessLogQueue()
        {
            using (StreamWriter file = new StreamWriter(_filePath, true))
            {
                foreach (var entry in _logQueue.GetConsumingEnumerable())
                {
                    file.Write(entry);
                    file.Flush();
                }
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _logQueue.CompleteAdding();
                _loggingTask.Wait();
                _logQueue.Dispose();
            }
        }
    }
}
