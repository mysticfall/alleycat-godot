using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public abstract class ConsoleCommand : IConsoleCommand
    {
        public abstract string Key { get; }

        public abstract string Description { get; }

        protected SceneTree SceneTree { get; }

        protected ConsoleCommand([NotNull] SceneTree sceneTree)
        {
            Ensure.Any.IsNotNull(sceneTree, nameof(sceneTree));

            SceneTree = sceneTree;
        }

        public abstract void Execute(string[] args, ICommandConsole console);

        public virtual void DisplayUsage(ICommandConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            var highlight = new TextStyle(console.HighlightColor);

            console
                .Write("[").Write(SceneTree.Tr("console.usage")).WriteLine("]")
                .NewLine()
                .Write("> ").WriteLine(Key, highlight)
                .WriteLine(Description);
        }
    }
}
