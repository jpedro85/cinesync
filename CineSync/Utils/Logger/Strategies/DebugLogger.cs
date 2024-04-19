using CineSync.Utils.Logger.Enums;
using System.Diagnostics;

namespace CineSync.Utils.Logger.Strategies
{
    public class DebugLogger : ILoggerStrategy
    {
        public void Log(string message, LogTypes? type)
        {
            Debug.WriteLine(message);
        }
    }
}
