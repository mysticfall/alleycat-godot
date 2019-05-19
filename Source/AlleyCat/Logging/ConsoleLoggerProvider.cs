using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.UI.Console;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    [ProviderAlias("Game")]
    public class ConsoleLoggerProvider : LoggerProvider
    {
        [Node]
        public Option<IConsole> Console { get; set; }

        [Export, UsedImplicitly] private NodePath _console = "../../UI/Console";

        protected override ILogger DoCreateLogger(string categoryName)
        {
            Option<IConsole> FindConsole() =>
                Console.IsSome ? Console : Console = this.FindComponent<IConsole>(_console);

            return new ConsoleLogger(
                categoryName,
                FindConsole,
                MinimumLevel,
                CategorySegments,
                ShowId);
        }
    }
}
