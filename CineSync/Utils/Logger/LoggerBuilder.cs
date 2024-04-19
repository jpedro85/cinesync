using CineSync.Utils.Logger.Strategies;
using CineSync.Utils.Logger.Decorators;

namespace CineSync.Utils.Logger
{
    public class LoggerBuilder
    {
        private CompositeLogger _compositeLogger = new CompositeLogger();
        private ILoggerStrategy? _currentLogger;

        public LoggerBuilder UseConsoleLogging()
        {
            _currentLogger = new ConsoleLogger();
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        public LoggerBuilder UseFileLogging(string filePath)
        {
            _currentLogger = new FileLogger(filePath);
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        public LoggerBuilder UseDebugLogging()
        {
            _currentLogger = new DebugLogger();
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        public LoggerBuilder UseTraceDebugging()
        {
            _currentLogger = new TraceLogger();
            _compositeLogger.AddLogger(_currentLogger);
            return this;
        }

        public LoggerBuilder AddTimeStamp()
        {
            if (_currentLogger != null)
            {
                _currentLogger = new TimeStampDecorator(_currentLogger);
                byte lastElement = (byte)(_compositeLogger.Loggers.Count - 1);
                _compositeLogger.Loggers[lastElement] = _currentLogger;
            }
            return this;
        }

        public LoggerBuilder AddUpperCase()
        {
            if (_currentLogger != null)
            {
                _currentLogger = new UpperCaseDecorator(_currentLogger);
                byte lastElement = (byte)(_compositeLogger.Loggers.Count - 1);
                _compositeLogger.Loggers[lastElement] = _currentLogger;
            }
            return this;
        }

        public ILoggerStrategy Build() {
            return _compositeLogger;
        }

    }
}
