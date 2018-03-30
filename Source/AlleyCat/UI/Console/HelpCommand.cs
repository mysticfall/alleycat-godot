using System.Linq;
using EnsureThat;
using Godot;

namespace AlleyCat.UI.Console
{
    public class HelpCommand : ConsoleCommand
    {
        public const string Command = "help";

        public override string Key => Command;

        public override string Description => SceneTree.Tr("console.command.help");

        public HelpCommand(SceneTree sceneTree) : base(sceneTree)
        {
        }

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
                            string.Format(SceneTree.Tr("console.error.command.invalid"), name),
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
                .Write("[").Write(SceneTree.Tr("console.usage")).WriteLine("]")
                .NewLine()
                .Write("> ").WriteLine(Key, highlight)
                .WriteLine(SceneTree.Tr("console.command.help.self"))
                .Write("> ").WriteLine("help list", highlight)
                .WriteLine(SceneTree.Tr("console.command.help.list"))
                .Write("> ").WriteLine("help <command>", highlight)
                .WriteLine(SceneTree.Tr("console.command.help.command"));
        }

        public void DisplayCommandList(IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            var highlight = new TextStyle(console.HighlightColor);

            console
                .Write("[").Write(SceneTree.Tr("console.commands")).WriteLine("]")
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
