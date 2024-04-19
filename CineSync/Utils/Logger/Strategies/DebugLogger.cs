using System.Diagnostics;

namespace CineSync.Utils.Logger.Strategies
{
    public class DebugLogger : ILoggerStrategy
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
