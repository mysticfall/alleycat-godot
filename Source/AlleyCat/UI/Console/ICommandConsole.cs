using System.Collections.Generic;

namespace AlleyCat.UI.Console
{
    public interface ICommandConsole : IConsole
    {
        IEnumerable<IConsoleCommand> SupportedCommands { get; }

        void Execute(string command, params string[] arguments);
    }
}
