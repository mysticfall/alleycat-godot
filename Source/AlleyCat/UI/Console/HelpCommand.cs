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
                        var message = string.Format(SceneTree.Tr("console.error.command.invalid"), name);

                        Console.Warning(message).NewLine();
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
            Console
                .Text("[").Text(SceneTree.Tr("console.usage")).Text("]").NewLine()
                .NewLine()
                .Text("> ").Highlight(Key).NewLine()
                .Text(SceneTree.Tr("console.command.help.self")).NewLine()
                .Text("> ").Highlight("help list").NewLine()
                .Text(SceneTree.Tr("console.command.help.list")).NewLine()
                .Text("> ").Highlight("help <command>").NewLine()
                .Text(SceneTree.Tr("console.command.help.command")).NewLine();
        }

        public void DisplayCommandList()
        {
            Console
                .Text("[").Text(SceneTree.Tr("console.commands")).Text("]").NewLine()
                .NewLine();

            foreach (var command in Console.SupportedCommands)
            {
                Console
                    .Highlight(command.Key).Text(" - ").Text(command.Description).NewLine();
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
