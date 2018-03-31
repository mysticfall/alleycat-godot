using Godot;

namespace AlleyCat.UI.Console
{
    public class ClearCommand : ConsoleCommand
    {
        public const string Command = "clear";

        public override string Key => Command;

        public override string Description => SceneTree.Tr("console.command.clear");

        public ClearCommand(ICommandConsole console, SceneTree sceneTree) : base(console, sceneTree)
        {
        }

        public override void Execute(string[] args) => Console.Clear();
    }
}
