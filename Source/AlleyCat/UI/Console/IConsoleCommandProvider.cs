using System.Collections.Generic;

namespace AlleyCat.UI.Console
{
    public interface IConsoleCommandProvider
    {
        IEnumerable<IConsoleCommand> CreateCommands(ICommandConsole console);
    }
}
