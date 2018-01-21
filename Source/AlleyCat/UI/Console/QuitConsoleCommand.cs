using EnsureThat;

namespace AlleyCat.UI.Console
{
    public class QuitConsoleCommand : ConsoleCommand
    {
        public const string Command = "quit";

        public override string Key => Command;

        public override string Description { get; } = "Exit the game immediately.";

        public override void Execute(string[] args, IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            GetTree().Quit();
        }
    }
}
