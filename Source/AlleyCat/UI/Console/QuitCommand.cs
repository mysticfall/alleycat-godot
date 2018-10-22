using Godot;
using LanguageExt;

namespace AlleyCat.UI.Console
{
    public class QuitCommand : ConsoleCommand
    {
        public const string Command = "quit";

        public override string Key => Command;

        public override Option<string> Description => SceneTree.Tr("console.command.quit");

        public QuitCommand(ICommandConsole console, SceneTree sceneRoot) : base(console, sceneRoot)
        {
        }

        public override void Execute(params string[] args) => SceneTree.Quit();
    }
}
