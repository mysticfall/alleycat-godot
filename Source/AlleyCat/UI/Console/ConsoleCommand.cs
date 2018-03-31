using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public abstract class ConsoleCommand : IConsoleCommand
    {
        public abstract string Key { get; }

        public abstract string Description { get; }

        public ICommandConsole Console { get; }

        protected SceneTree SceneTree { get; }

        protected ConsoleCommand([NotNull] ICommandConsole console, SceneTree sceneTree)
        {
            Ensure.Any.IsNotNull(console, nameof(console));
            Ensure.Any.IsNotNull(sceneTree, nameof(sceneTree));

            Console = console;
            SceneTree = sceneTree;
        }

        public abstract void Execute(string[] args);

        public virtual void DisplayUsage()
        {
            var highlight = new TextStyle(Console.HighlightColor);

            Console
                .Write("[").Write(SceneTree.Tr("console.usage")).WriteLine("]")
                .NewLine()
                .Write("> ").WriteLine(Key, highlight)
                .WriteLine(Description);
        }
    }
}
