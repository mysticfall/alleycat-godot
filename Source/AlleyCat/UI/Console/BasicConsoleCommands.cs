using System.Collections.Generic;

namespace AlleyCat.UI.Console
{
    public class BasicConsoleCommands : ConsoleCommandProvider
    {
        protected override IEnumerable<IConsoleCommand> CreateCommands()
        {
            var sceneTree = GetTree();

            return new IConsoleCommand[]
            {
                new ClearCommand(sceneTree),
                new HelpCommand(sceneTree),
                new QuitCommand(sceneTree)
            };
        }
    }
}
