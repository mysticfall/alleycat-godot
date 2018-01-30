using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.UI.Console
{
    public class QuitCommand : ConsoleCommand
    {
        public const string Command = "quit";

        public override string Key => Command;

        public override string Description { get; } = "Exit the game immediately.";

        [NotNull]
        protected SceneTree SceneRoot { get; }

        public QuitCommand([NotNull] SceneTree sceneRoot)
        {
            Ensure.Any.IsNotNull(sceneRoot, nameof(sceneRoot));

            SceneRoot = sceneRoot;
        }

        public override void Execute(string[] args, IConsole console) => SceneRoot.Quit();
    }
}
