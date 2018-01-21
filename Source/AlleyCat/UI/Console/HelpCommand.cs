using System.Linq;
using EnsureThat;

namespace AlleyCat.UI.Console
{
    public class HelpCommand : ConsoleCommand
    {
        public const string Command = "help";

        public override string Key => Command;

        public override string Description { get; } = "Show help information of console commands.";

        public override void Execute(string[] args, IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            if (args == null || args.Length != 1)
            {
                DisplayUsage(console);
            }
            else
            {
                var name = args[0];

                if (name == "list")
                {
                    DisplayCommandList(console);
                }
                else
                {
                    var command = console.SupportedCommands.FirstOrDefault(c => c.Key == name);

                    if (command == null)
                    {
                        console.WriteLine(
                            $"No such command exists: '{name}'.",
                            new TextStyle(console.WarningColor));
                    }
                    else
                    {
                        command.DisplayUsage(console);
                    }
                }
            }
        }

        public override void DisplayUsage(IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            var highlight = new TextStyle(console.HighlightColor);

            console
                .WriteLine("[Usage]")
                .NewLine()
                .Write("> ").WriteLine(Key, highlight)
                .WriteLine("Show this instruction.")
                .Write("> ").WriteLine("help list", highlight)
                .WriteLine("Display list of available commands.")
                .Write("> ").WriteLine("help <command>", highlight)
                .WriteLine("Display help message for the specified command.");
        }

        public void DisplayCommandList(IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            var highlight = new TextStyle(console.HighlightColor);

            console
                .WriteLine("[Commands]")
                .NewLine();

            foreach (var command in console.SupportedCommands)
            {
                console
                    .Write(command.Key, highlight)
                    .Write(" - ")
                    .Write(command.Description)
                    .NewLine();
            }
        }
    }
}
