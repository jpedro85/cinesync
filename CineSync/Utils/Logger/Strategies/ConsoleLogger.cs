using CineSync.Utils.Logger.Enums;

namespace CineSync.Utils.Logger.Strategies
{
    public class ConsoleLogger : ILoggerStrategy
    {
        public void Log(string message, LogTypes? type)
        {
            Console.WriteLine(message);

        }
    }
}
