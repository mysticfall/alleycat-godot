using System.Collections.Generic;

namespace AlleyCat.UI.Console
{
    public class BasicConsoleCommands : ConsoleCommandProvider
    {
        protected override IEnumerable<IConsoleCommand> CreateCommands()
        {
            return new IConsoleCommand[]
            {
                new ClearCommand(),
                new HelpCommand(),
                new QuitCommand(GetTree())
            };
        }
    }
}
