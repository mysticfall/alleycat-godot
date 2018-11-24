using AlleyCat.Autowire;
using AlleyCat.UI.Console;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    public class ConsoleLoggerProvider : LoggerProvider
    {
        [Service]
        public DebugConsole Console { get; private set; }

        protected override ILogger DoCreateLogger(string categoryName) =>
            new ConsoleLogger(categoryName, Console, CategorySegments, ShowId);
    }
}
