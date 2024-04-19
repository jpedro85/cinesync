using System.Diagnostics;

namespace CineSync.Utils.Logger.Strategies
{
    public class TraceLogger : ILoggerStrategy
    {
        public void Log(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
