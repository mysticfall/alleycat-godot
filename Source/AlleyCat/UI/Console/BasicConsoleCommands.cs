using System.Collections.Generic;
using AlleyCat.Autowire;
using EnsureThat;

namespace AlleyCat.UI.Console
{
    [Singleton(typeof(IConsoleCommandProvider))]
    public class BasicConsoleCommands : AutowiredNode, IConsoleCommandProvider
    {
        public IEnumerable<IConsoleCommand> CreateCommands(ICommandConsole console)
        {
            Ensure.Any.IsNotNull(console, nameof(console));

            var sceneTree = GetTree();

            return new IConsoleCommand[]
            {
                new ClearCommand(console, sceneTree),
                new HelpCommand(console, sceneTree),
                new QuitCommand(console, sceneTree)
            };
        }
    }
}
