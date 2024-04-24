using CineSync.Core.Logger.Decorators;
using CineSync.Core.Logger.Strategies;

namespace CineSync.Core.Logger
{
    // <summary>
    /// Builds a flexible logger by using a combination of different logging strategies and decorators.
    /// </summary>
    public class LoggerBuilder
    {
        private CompositeLogger _compositeLogger = new CompositeLogger();
        private ILoggerStrategy? _currentLogger;

        /// <summary>
        /// Configures the logger builder to include console logging in the logging operations.
        /// </summary>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder UseConsoleLogging()
        {
            _currentLogger = new ConsoleLogger();
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        /// <summary>
        /// Configures the logger builder to include file logging in the logging operations.
        /// File path is required to direct the log output to a specific file.
        /// </summary>
        /// <param name="filePath">Path to the log file.</param>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder UseFileLogging(string filePath)
        {
            _currentLogger = new FileLogger(filePath);
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        /// <summary>
        /// Configures the logger builder to include debug logging in the logging operations.
        /// </summary>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder UseDebugLogging()
        {
            _currentLogger = new DebugLogger();
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        /// <summary>
        /// Configures the logger builder to include trace logging in the logging operations.
        /// </summary>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder UseTraceDebugging()
        {
            _currentLogger = new TraceLogger();
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        /// <summary>
        /// Adds a timestamp decorator to the current logger configuration.
        /// This decorator prefixes log messages with the current timestamp.
        /// </summary>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder AddTimeStamp()
        {
            if (_currentLogger != null)
            {
                _currentLogger = new TimeStamp(_currentLogger);
                byte lastElement = (byte)(_compositeLogger.Loggers.Count - 1);
                _compositeLogger.Loggers[lastElement] = _currentLogger;
            }
            return this;
        }

        /// <summary>
        /// Adds an upper case decorator to the current logger configuration.
        /// This decorator converts all log messages to upper case.
        /// </summary>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder AddUpperCase()
        {
            if (_currentLogger != null)
            {
                _currentLogger = new UpperCase(_currentLogger);
                byte lastElement = (byte)(_compositeLogger.Loggers.Count - 1);
                _compositeLogger.Loggers[lastElement] = _currentLogger;
            }
            return this;
        }

        /// <summary>
        /// Adds a log type decorator to the current logger configuration.
        /// This decorator prefixes log messages with the log type, indicating the severity or category of the log message.
        /// Utilizes the <see cref="LogTypes"/> enum to tag log entries with types such as INFO, DEBUG, WARN, or ERROR.
        /// </summary>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder AddType()
        {
            if (_currentLogger != null)
            {
                _currentLogger = new LogType(_currentLogger);
                byte lastElement = (byte)(_compositeLogger.Loggers.Count - 1);
                _compositeLogger.Loggers[lastElement] = _currentLogger;
            }
            return this;
        }

        /// <summary>
        /// Makes so that all the Text is plaintext and there is no escape sequences
        /// This decorator prefixes log messages with the log type, indicating the severity or category of the log message.
        /// Utilizes the <see cref="LogTypes"/> enum to tag log entries with types such as INFO, DEBUG, WARN, or ERROR.
        /// </summary>
        /// <returns>The same LoggerBuilder instance for chaining configuration calls.</returns>
        public LoggerBuilder AsPlainText()
        {
            if (_currentLogger != null)
            {
                _currentLogger = new PlainText(_currentLogger);
                byte lastElement = (byte)(_compositeLogger.Loggers.Count - 1);
                _compositeLogger.Loggers[lastElement] = _currentLogger;
            }
            return this;
        }

        /// <summary>
        /// Completes the configuration and builds the composite logger containing all configured loggers and decorators.
        /// </summary>
        /// <returns>An instance of ILoggerStrategy representing the composed logger.</returns>
        public ILoggerStrategy Build()
        {
            return _compositeLogger;
        }

    }
}
