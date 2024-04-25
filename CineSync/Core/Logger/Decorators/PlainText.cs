using System.Text.RegularExpressions;
using CineSync.Core.Logger.Enums;

namespace CineSync.Core.Logger.Decorators
{
    public class PlainText : LoggerDecorator
    {
        public PlainText(ILoggerStrategy wrappedLogger) : base(wrappedLogger) { }

        public override void Log(string message, LogTypes? type)
        {
            // Regex pattern that removes the escape sequences
            string ansiPattern = @"\x1B\[\d+;?\d*;?\d*;?\d*;?\d*m";

            // Remove all ANSI escape sequences from the message
            string plaintextMessage = Regex.Replace(message, ansiPattern, string.Empty);
            base.Log(plaintextMessage, type);
        }
    }
}

