using EnsureThat;
using Godot;

namespace AlleyCat.UI.Console
{
    public class ClearCommand : ConsoleCommand
    {
        public const string Command = "clear";

        public override string Key => Command;

        public override string Description => SceneTree.Tr("console.command.clear");

        public ClearCommand(SceneTree sceneTree) : base(sceneTree)
        {
        }

        public override void Execute(string[] args, IConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            console.Clear();
        }
    }
}
