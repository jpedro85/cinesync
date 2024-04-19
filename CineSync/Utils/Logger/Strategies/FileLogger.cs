using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Strategies
{
    public class FileLogger : ILoggerStrategy
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message, LogTypes? type)
        {
            // WARN: Check if this is doesnt have problems on multiple request at the same time
            File.AppendAllText(_filePath, message + Environment.NewLine);
        }
    }
}
