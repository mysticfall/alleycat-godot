using Godot;

namespace AlleyCat.UI.Console
{
    public class QuitCommand : ConsoleCommand
    {
        public const string Command = "quit";

        public override string Key => Command;

        public override string Description => SceneTree.Tr("console.command.quit");

        public QuitCommand(SceneTree sceneRoot) : base(sceneRoot)
        {
        }

        public override void Execute(string[] args, IConsole console) => SceneTree.Quit();
    }
}
