using AlleyCat.Autowire;
using EnsureThat;

namespace AlleyCat.UI.Console
{
    [Singleton(typeof(IConsoleCommand))]
    public abstract class ConsoleCommand : AutowiredNode, IConsoleCommand
    {
        public abstract string Key { get; }

        public abstract string Description { get; }

        public abstract void Execute(string[] args, IConsole console);

        public virtual void DisplayUsage(IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            var highlight = new TextStyle(console.HighlightColor);

            console
                .WriteLine("[Usage]")
                .NewLine()
                .Write("> ").WriteLine(Key, highlight)
                .WriteLine(Description);
        }
    }
}
