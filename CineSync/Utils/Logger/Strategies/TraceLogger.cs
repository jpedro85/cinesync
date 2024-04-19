using CineSync.Utils.Logger.Enums;
using System.Diagnostics;

namespace CineSync.Utils.Logger.Strategies
{
    public class TraceLogger : ILoggerStrategy
    {
        public void Log(string message, LogTypes? type)
        {
            Trace.WriteLine(message);
        }
    }
}
