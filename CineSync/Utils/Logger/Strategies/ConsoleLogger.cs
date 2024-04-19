namespace CineSync.Utils.Logger.Strategies
{
    public class ConsoleLogger : ILoggerStrategy
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
