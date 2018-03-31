using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AlleyCat.UI.Console
{
    public class HelpCommand : ConsoleCommand, IAutoCompletionSupport
    {
        public const string Command = "help";

        public override string Key => Command;

        public override string Description => SceneTree.Tr("console.command.help");

        public HelpCommand(ICommandConsole console, SceneTree sceneTree) : base(console, sceneTree)
        {
        }

        public override void Execute(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                DisplayUsage();
            }
            else
            {
                var name = args[0];

                if (name == "list")
                {
                    DisplayCommandList();
                }
                else
                {
                    var command = Console.SupportedCommands.FirstOrDefault(c => c.Key == name);

                    if (command == null)
                    {
                        Console.WriteLine(
                            string.Format(SceneTree.Tr("console.error.command.invalid"), name),
                            new TextStyle(Console.WarningColor));
                    }
                    else
                    {
                        command.DisplayUsage();
                    }
                }
            }
        }

        public override void DisplayUsage()
        {
            var highlight = new TextStyle(Console.HighlightColor);

            Console
                .Write("[").Write(SceneTree.Tr("console.usage")).WriteLine("]")
                .NewLine()
                .Write("> ").WriteLine(Key, highlight)
                .WriteLine(SceneTree.Tr("console.command.help.self"))
                .Write("> ").WriteLine("help list", highlight)
                .WriteLine(SceneTree.Tr("console.command.help.list"))
                .Write("> ").WriteLine("help <command>", highlight)
                .WriteLine(SceneTree.Tr("console.command.help.command"));
        }

        public void DisplayCommandList()
        {
            var highlight = new TextStyle(Console.HighlightColor);

            Console
                .Write("[").Write(SceneTree.Tr("console.commands")).WriteLine("]")
                .NewLine();

            foreach (var command in Console.SupportedCommands)
            {
                Console
                    .Write(command.Key, highlight)
                    .Write(" - ")
                    .Write(command.Description)
                    .NewLine();
            }
        }

        public IEnumerable<string> SuggestCandidates(string text)
        {
            var commands = Console.SupportedCommands.Select(c => c.Key).Where(c => c != Key).ToList();

            if (string.IsNullOrWhiteSpace(text))
            {
                commands.Insert(0, "list");

                return commands;
            }

            return "list".StartsWith(text) ? new[] {"list"} : commands.Where(c => c.StartsWith(text));
        }
    }
}
