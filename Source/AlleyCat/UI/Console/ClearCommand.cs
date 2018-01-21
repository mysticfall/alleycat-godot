using EnsureThat;

namespace AlleyCat.UI.Console
{
    public class ClearCommand : ConsoleCommand
    {
        public const string Command = "clear";

        public override string Key => Command;

        public override string Description { get; } = "Clear the console.";

        public override void Execute(string[] args, IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            console.Clear();
        }
    }
}
