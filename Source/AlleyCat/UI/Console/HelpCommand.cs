using System.Collections.Generic;
using System.Linq;
using Godot;
using LanguageExt;

namespace AlleyCat.UI.Console
{
    public class HelpCommand : ConsoleCommand, IAutoCompletionSupport
    {
        public const string Command = "help";

        public override string Key => Command;

        public override Option<string> Description => SceneTree.Tr("console.command.help");

        public HelpCommand(ICommandConsole console, SceneTree sceneTree) : base(console, sceneTree)
        {
        }

        public override void Execute(params string[] args)
        {
            args.HeadOrNone().Match(name =>
                {
                    if (name == "list")
                    {
                        DisplayCommandList();
                    }
                    else
                    {
                        var command = Console.SupportedCommands.Find(c => c.Key == name);

                        command.BiIter(
                            c => c.DisplayUsage(),
                            () =>
                            {
                                var message = string.Format(SceneTree.Tr("console.error.command.invalid"), name);

                                Console.Warning(message).NewLine();
                            });
                    }
                },
                DisplayUsage);
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
                Console.Highlight(command.Key);

                command.Description.Iter(d => Console.Text(" - ").Text(d));

                Console.NewLine();
            }
        }

        public IEnumerable<string> SuggestCandidates(Option<string> text)
        {
            var commands = Console.SupportedCommands.Select(c => c.Key).Where(c => c != Key).ToList();

            if (text.Exists(string.IsNullOrWhiteSpace))
            {
                commands.Insert(0, "list");

                return commands;
            }

            return text.AsEnumerable()
                .Bind(v => "list".StartsWith(v) ? new[] {"list"} : commands.Where(c => c.StartsWith(v)));
        }
    }
}
