using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.UI.Console
{
    public abstract class ConsoleCommand : IConsoleCommand
    {
        public abstract string Key { get; }

        public abstract Option<string> Description { get; }

        public ICommandConsole Console { get; }

        protected SceneTree SceneTree { get; }

        protected ConsoleCommand(ICommandConsole console, SceneTree sceneTree)
        {
            Ensure.That(console, nameof(console)).IsNotNull();
            Ensure.That(sceneTree, nameof(sceneTree)).IsNotNull();

            Console = console;
            SceneTree = sceneTree;
        }

        public abstract void Execute(params string[] args);

        public virtual void DisplayUsage()
        {
            Console
                .Text("[").Text(SceneTree.Tr("console.usage")).Text("]").NewLine()
                .NewLine()
                .Text("> ").Highlight(Key).NewLine();

            Description.Iter(d => Console.Text(d).NewLine());
        }
    }
}
